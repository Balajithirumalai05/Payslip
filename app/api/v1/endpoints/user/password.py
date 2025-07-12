from fastapi import APIRouter, Request, Form, Depends, BackgroundTasks
from fastapi.responses import HTMLResponse
from sqlalchemy.orm import Session
from app.db.session import get_db
from fastapi.templating import Jinja2Templates
from app.core.auth import get_current_user
from app.services.password_service import (
    change_password as change_password_service,
    forgot_password as forgot_password_service,
    reset_password as reset_password_service,
    validate_password_reset_token
)

router = APIRouter()
templates = Jinja2Templates(directory="app/templates")

@router.get("/change-password", response_class=HTMLResponse)
async def changepass(request: Request, current_user = Depends(get_current_user)):
    return templates.TemplateResponse("change_password.html", {"request": request})

@router.post("/change-password", response_class=HTMLResponse)
async def handle_change_password(
    request:Request,
    current_password = Form(...),
    new_password = Form(...),
    confirm_password = Form(...),
    db = Depends(get_db),
    current_user = Depends(get_current_user)
):
    return await change_password_service(
        request=request,
        current_password=current_password,
        new_password=new_password,
        confirm_password=confirm_password,
        db=db,
        current_user=current_user
    )

@router.get("/forgot-password")
async def forgotPass(request:Request):
    return templates.TemplateResponse("forgot_password.html", {"request": request})

@router.post("/forgot-password", response_class=HTMLResponse)
async def handle_forgot_password(
    request:Request,
    email = Form(...),
    db = Depends(get_db),
    background_tasks = BackgroundTasks()
):
    return await forgot_password_service(
        request=request,
        email=email,
        db=db,
        background_tasks=background_tasks
    )

@router.get("/reset-password")
async def resetPass(request:Request, token):
    if not validate_password_reset_token(token):
        return templates.TemplateResponse(
            "reset.html",
            {"request": request, "error": "Invalid or expired token"}
        )
    return templates.TemplateResponse("reset.html", {"request": request, "token": token})

@router.post("/reset-password", response_class=HTMLResponse)
async def handle_reset_password(
    request:Request,
    token = Form(...),
    new_password = Form(...),
    confirm_password = Form(...),
    db = Depends(get_db)
):
    return await reset_password_service(
        request=request,
        token=token,
        new_password=new_password,
        confirm_password=confirm_password,
        db=db
    )