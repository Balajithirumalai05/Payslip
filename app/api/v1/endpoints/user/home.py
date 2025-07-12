from fastapi import APIRouter, Request, Form, Depends,HTTPException
from sqlalchemy.orm import Session
from app.db.session import get_db
from sqlalchemy import func
import app.models.models as models
from fastapi.templating import Jinja2Templates
from pydantic import BaseModel
from app.core.auth import get_current_user
from datetime import datetime,timedelta
from typing import List,Dict

from app.services.employee_service import get_employee_by_id, get_employee_name, get_profile_by_id

router = APIRouter()
templates = Jinja2Templates(directory="app/templates")







@router.get("/home")
async def home(request: Request, current_user: dict = Depends(get_current_user), db: Session = Depends(get_db)):
    user_id = current_user.get("user_id")
    username = get_employee_name(user_id, db)
    
    # Fetch leave balances for the current user
    employee = get_employee_by_id(user_id, db)
    if not employee:
        raise HTTPException(status_code=404, detail="Employee not found")
    
    profile= get_profile_by_id(user_id, db)

    
    

    return templates.TemplateResponse("home.html",{
        "request": request,
        "username": username,
  
        "profile":profile,
    })


