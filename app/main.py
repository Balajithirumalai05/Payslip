from fastapi import FastAPI, Depends, HTTPException,Request,Form,Cookie,UploadFile,File,BackgroundTasks
from sqlalchemy.orm import Session
from sqlalchemy import func
from datetime import datetime, timedelta,date,time
from app.api.v1.endpoints.admin import admin, admin_home, admin_payslip
from app.api.v1.endpoints.user import  employee, home,  password, payroll, profiles
import app.models.models as models
import app.core.auth as auth
from app.core.auth import create_access_token,create_user, get_current_user,authenticate_user,authenticate_admin,bcrypt_context,SECRET_KEY,ALGORITHM
from app.db.session import engine,SessionLocal, get_db
from pydantic import BaseModel
from typing import Annotated,List,Dict
from fastapi.templating import Jinja2Templates
from fastapi.staticfiles import StaticFiles
from fastapi.responses import RedirectResponse,HTMLResponse
import os
from dotenv import load_dotenv
from app.utils.send_mail import send_reset_email
import os
from app.models.models import Attendance, User, Admin, PayRoll
from app.db.session import get_db  # adjust this import as per your app





app=FastAPI()
app.include_router(auth.router)
app.include_router(employee.router)
app.include_router(home.router)
app.include_router(password.router)
app.include_router(payroll.router)
app.include_router(profiles.router)
app.include_router(admin.router)
app.include_router(admin_home.router)
app.include_router(admin_payslip.router)

models.Base.metadata.create_all(bind=engine)
templates=Jinja2Templates(directory="app/templates")
app.mount('/app/static',StaticFiles(directory="app/static"),name="static")
app.mount("/app/uploads", StaticFiles(directory="app/uploads"), name="uploads")
load_dotenv()





@app.get("/",response_class=HTMLResponse)
async def login(request:Request):
    return templates.TemplateResponse("login.html",{"request":request})



@app.post("/login", response_class=HTMLResponse)
async def user_login(request: Request, email: str = Form(...), password: str = Form(...), db: Session = Depends(get_db)):
    user = authenticate_user(email, password, db)
    if not user:
        return templates.TemplateResponse("login.html", {"request": request, "error": "Invalid username or password"})

    # ✅ Save login entry
    attendance = Attendance(user_id=user.id)
    db.add(attendance)
    db.commit()
    db.refresh(attendance)

    # ✅ Set cookies
    token = create_access_token(user.email, user.id, timedelta(minutes=1440))
    response = RedirectResponse(url="/home", status_code=303)
    response.set_cookie(key="access_token", value=token, httponly=True, secure=True)
    response.set_cookie(key="attendance_id", value=str(attendance.id), httponly=True, secure=True)

    return response



@app.get("/admin/login", response_class=HTMLResponse)
async def admin_login_page(request: Request):
    return templates.TemplateResponse("admin_login.html", {"request": request})
    
@app.post("/admin/login", response_class=HTMLResponse)
async def admin_login(
    request: Request, 
    email: str = Form(...), 
    password: str = Form(...), 
    db: Session = Depends(get_db)
):
    admin = authenticate_admin(email, password, db)
    if not admin:
        return templates.TemplateResponse("admin_login.html", 
            {"request": request, "error": "Invalid credentials"})
    
    token = create_access_token(admin.email, admin.id, timedelta(minutes=1440))
    response = RedirectResponse(url="/admin/dashboard", status_code=303)
    # Properly indented cookie setting
    response.set_cookie(
        key="admin_access_token",
        value=token,
        httponly=True,
        secure=True
    )
    return response# Set token as cookie

@app.post("/logout")
async def logout(request: Request, db: Session = Depends(get_db)):
    # Step 1: Get attendance_id from cookie
    attendance_id = request.cookies.get("attendance_id")

    if attendance_id:
        attendance = db.query(Attendance).filter(Attendance.id == int(attendance_id)).first()
        if attendance and not attendance.logout_time:
            attendance.logout_time = datetime.utcnow()
            time_diff = attendance.logout_time - attendance.login_time
            attendance.hours_worked = round(time_diff.total_seconds() / 3600, 2)
            db.commit()

    # Step 2: Clear cookies and redirect
    response = RedirectResponse(url="/", status_code=303)
    response.delete_cookie(key="access_token", path="/")
    response.delete_cookie(key="attendance_id", path="/")
    return response

@app.post("/admin/logout")
async def logout():
    response = RedirectResponse(url="/admin/login", status_code=303)
    response.delete_cookie(key="admin_access_token",path="/")
    return response

