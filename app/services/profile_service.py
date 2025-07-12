from fastapi import Depends,HTTPException, Request
from sqlalchemy.orm import Session
from app.db.session import get_db
from datetime import datetime,timedelta
import app.models.models as models
from fastapi.templating import Jinja2Templates
from sqlalchemy import func
from app.core.auth import get_current_user
import numpy as np

templates = Jinja2Templates(directory="app/templates")



async def calculate_total_leave_days(current_user:dict=Depends(get_current_user), db=Depends(get_db)) -> int:
    employee_id=current_user["user_id"]
    total_leave_days = (
        db.query(func.sum(models.LeaveRequest.leave_days))
        .filter(models.LeaveRequest.employee_id == employee_id, models.LeaveRequest.status == "approve")
        .scalar()
    )
    return total_leave_days if total_leave_days else 0


async def calculate_workdays(date_of_join):
    """Calculate the number of working days (excluding weekends) from date_of_join to today."""
    today = datetime.today().date()
    join_date = date_of_join#.date()  
    
    if join_date > today:
        return 0  

    # Generate an array of all dates between join_date and today
    all_dates = np.arange(join_date, today + timedelta(days=1), dtype="datetime64[D]")

    # Count only weekdays (Monday to Friday)
    workdays = np.is_busday(all_dates)
    
    return int(workdays.sum())

async def get_profile(current_user: dict = Depends(get_current_user), db: Session = Depends(get_db)):
    employee_id = current_user["user_id"]
    
   
    profile = db.query(models.Profile).filter(models.Profile.id == employee_id).first()
    # print(profile.role)
    if profile is None:
        raise HTTPException(status_code=404, detail="Profile not found")
    
    # Extracting data from the row
    total_leave_days = await calculate_total_leave_days(current_user, db)
    work_days =await calculate_workdays(profile.date_of_join)
    return {
        "name": profile.name,
        "role": profile.role,
        "email": profile.email,
        "age": profile.age,
        "date_of_birth": profile.date_of_birth.strftime("%Y-%m-%d"),
        "date_of_join": profile.date_of_join.strftime("%Y-%m-%d"),
        "phone_number": profile.phone_number,
        "working_days": 220, 
        "days_in_company": work_days, 
        "leave_taken": total_leave_days,
        "active_projects": 4,
        "performance": 95,
        "image_url": profile.image_url
    }

async def view_profile(request: Request, current_user: dict = Depends(get_current_user),db: Session = Depends(get_db)):
    employee_id = current_user["user_id"]
    
        
    profile=db.query(models.Profile).filter(models.Profile.id == employee_id).first()
    
    if profile is None:
        raise HTTPException(status_code=404, detail="Profile not found")
    
    
    return templates.TemplateResponse("profile.html", {
        "request": request,
        "profile": profile,
    })
