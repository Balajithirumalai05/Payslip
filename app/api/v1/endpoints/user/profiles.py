from fastapi import APIRouter, Request, Form, Depends,HTTPException
from sqlalchemy.orm import Session
from app.db.session import get_db
import app.models.models as models
from fastapi.templating import Jinja2Templates
from sqlalchemy import func
from app.core.auth import get_current_user
from app.services.profile_service import (get_profile as get_employee_profile,view_profile as view_employee_profile)

router = APIRouter()
templates = Jinja2Templates(directory="app/templates")


@router.get("/api/profile")
async def get_profile(current_user: dict = Depends(get_current_user), db: Session = Depends(get_db)):
    return await get_employee_profile(current_user=current_user,db=db)


@router.get("/profile")
async def view_profile(request: Request, current_user: dict = Depends(get_current_user),db: Session = Depends(get_db)):
    return await view_employee_profile(request=request,current_user=current_user,db=db)




