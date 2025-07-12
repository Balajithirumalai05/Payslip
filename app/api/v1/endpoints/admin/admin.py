from fastapi import APIRouter, Request, Form, Depends, HTTPException
from fastapi.templating import Jinja2Templates
from sqlalchemy.orm import Session

from app.core.auth import get_db
from app.services.admin_service import create_admin, is_admin_existing

router = APIRouter()
templates = Jinja2Templates(directory="app/templates")


@router.get("/admin/add")
async def get_admin(request: Request):
    return templates.TemplateResponse("addadmin.html", {"request": request})


@router.post("/admin/add")
async def add_admin(
    email: str = Form(...),
    password: str = Form(...),
    db: Session = Depends(get_db)
):
    if is_admin_existing(email, db):
        raise HTTPException(status_code=400, detail="Admin already exists")

    try:
        create_admin(email, password, db)
        return {"message": "Admin added successfully"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"An error occurred: {str(e)}")
