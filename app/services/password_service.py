from app.models import models
from fastapi import Request, HTTPException, Depends, Form, BackgroundTasks
from sqlalchemy.orm import Session
from app.db.session import get_db
from app.core.auth import get_current_user, bcrypt_context
from fastapi.templating import Jinja2Templates
import secrets
from app.utils.send_mail import send_reset_email

templates = Jinja2Templates(directory="app/templates")

async def change_password(
    request,
    current_password = Form(...),
    new_password = Form(...),
    confirm_password = Form(...),
    db = Depends(get_db),
    current_user = Depends(get_current_user)
):
    user_id = current_user['user_id']
    user = db.query(models.User).filter(models.User.id == user_id).first()
    
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    
    if not bcrypt_context.verify(current_password, user.hashed_password):
        return templates.TemplateResponse(
            "change_password.html", 
            {"request": request, "error": "Current password is incorrect"}
        )
    
    if new_password != confirm_password:
        return templates.TemplateResponse(
            "change_password.html", 
            {"request": request, "error": "New passwords do not match"}
        )
    
    user.hashed_password = bcrypt_context.hash(new_password)
    db.commit()
    
    return templates.TemplateResponse(
        "change_password.html", 
        {"request": request, "success": "Password changed successfully"}
    )

async def forgot_password(
    request,
    email = Form(...),
    db = Depends(get_db),
    background_tasks = None
):
    user = db.query(models.User).filter(models.User.email == email.lower().strip()).first()
    
    if not user:
        return templates.TemplateResponse(
            "forgot_password.html", 
            {"request": request, "error": "Email not found"}
        )
    
    reset_token = secrets.token_urlsafe(32)
    user.reset_token = reset_token
    db.commit()
    
    reset_link = f"http://localhost:8000/reset-password?token={reset_token}"
    

    send_reset_email(email, reset_link)

    return templates.TemplateResponse(
        "forgot_password.html", 
        {"request": request, "success": "Password reset email sent"}
    )

async def reset_password(
    request,
    token = Form(...),
    new_password = Form(...),
    confirm_password = Form(...),
    db = Depends(get_db)
):
    user = db.query(models.User).filter(models.User.reset_token == token).first()
    
    if not user:
        return templates.TemplateResponse(
            "reset.html", 
            {"request": request, "error": "Invalid or expired token"}
        )
    
    if new_password != confirm_password:
        return templates.TemplateResponse(
            "reset.html", 
            {"request": request, "error": "New passwords do not match"}
        )
    
    if len(new_password) < 8:
        return templates.TemplateResponse(
            "reset.html",
            {"request": request, "error": "Password must be at least 8 characters"}
        )
    
    user.hashed_password = bcrypt_context.hash(new_password)
    user.reset_token = None
    db.commit()
    
    return templates.TemplateResponse(
        "reset.html", 
        {"request": request, "success": "Password reset successfully"}
    )

async def validate_password_reset_token(token, db = Depends(get_db)):
    return db.query(models.User).filter(models.User.reset_token == token).first() is not None