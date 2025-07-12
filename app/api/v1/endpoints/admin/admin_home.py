from fastapi import APIRouter, Request, Depends
from fastapi.templating import Jinja2Templates
from app.core.auth import get_current_admin, get_db
from sqlalchemy.orm import Session #used for database access

from app.services import employee_service
from app.models.models import PayRoll

router = APIRouter()
templates = Jinja2Templates(directory="app/templates") #used for html rendering



@router.get("/admin/dashboard")
async def admin_home(
    request: Request,
    current_admin: dict = Depends(get_current_admin),
    db: Session = Depends(get_db)
):
    admin_id = current_admin["admin_id"]
    emp_count = employee_service.get_admin_dashboard_data(db)

  

    payrolls = db.query(PayRoll).order_by(PayRoll.id.desc()).limit(5).all()





    return templates.TemplateResponse("admin_home.html", {
        "request": request,
        "emp_count": emp_count,

    })
