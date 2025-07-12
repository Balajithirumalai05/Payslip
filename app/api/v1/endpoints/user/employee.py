# app/api/endpoints/employee.py

from fastapi import APIRouter, Request, Form, Depends, HTTPException
from fastapi.templating import Jinja2Templates
from sqlalchemy.orm import Session
from app.db.session import get_db
from app.core.auth import bcrypt_context
from app.services import employee_service
from app.core.auth import get_current_admin, get_db
from fastapi.responses import RedirectResponse


router = APIRouter()
templates = Jinja2Templates(directory="app/templates")

@router.post("/employee/add")
async def add_employee(
    name: str = Form(...),
    email: str = Form(...),
    password: str = Form(...),
    role: str = Form(...),
    annual_leave: int = Form(10),
    special_leave: int = Form(10),
    sick_leave: int = Form(10),
    public_leave: int = Form(10),
    gender: str = Form(...),
    phone_number: str = Form(...),
    age: int = Form(...),
    date_of_birth: str = Form(...),
    date_of_join: str = Form(...),
    blood_group: str = Form(None),
    address: str = Form(None),
    db: Session = Depends(get_db)
):
    return await employee_service.create_employee(
        db, name, email, password, role, annual_leave, special_leave, sick_leave,
        public_leave, gender, phone_number, age, date_of_birth, date_of_join,
        blood_group, address
    )
    return RedirectResponse(url="/employee/add?msg=added", status_code=303)


@router.get("/employee/add")
async def get_employee(request: Request,msg: str = None, current_admin: dict = Depends(get_current_admin), db=Depends(get_db)):
    return templates.TemplateResponse("addemployee.html", {"request": request, "msg": msg})
