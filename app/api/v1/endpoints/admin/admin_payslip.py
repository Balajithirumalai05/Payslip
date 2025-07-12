from fastapi import APIRouter, Request, Form, Depends, HTTPException
from fastapi.templating import Jinja2Templates
from fastapi.responses import RedirectResponse

from app.core.auth import get_current_admin, get_db
from app.services import payroll_service

router = APIRouter()
templates = Jinja2Templates(directory="app/templates")


@router.get("/emp-paylist")
async def emp_list(request: Request, current_admin: dict = Depends(get_current_admin), db=Depends(get_db)):
    admin_id = current_admin["admin_id"]
    emp_list = payroll_service.get_employee_list(db)
    return templates.TemplateResponse('pay_detail.html', {'request': request, 'empl': emp_list})


@router.post("/emp-paylist")
async def emp_details(request: Request, emp_id: int = Form(...), current_admin: dict = Depends(get_current_admin), db=Depends(get_db)):
    admin_id = current_admin["admin_id"]
    employee_data = payroll_service.get_employee_by_id(db, emp_id)
    return templates.TemplateResponse('editpay.html', {'request': request, 'emp': employee_data})


@router.post("/editpay")
async def edit_pay(
    request: Request,
    user_id: int = Form(...),
    basic: float = Form(...),
    hra: float = Form(...),
    other_allowance: float = Form(...),
    income_tax: float = Form(...),
    provident_fund: float = Form(...),
    current_admin: dict = Depends(get_current_admin),
    db=Depends(get_db)
):
    admin_id = current_admin["admin_id"]
    payroll_service.update_employee_pay_details(db, user_id, basic, hra, other_allowance, income_tax, provident_fund)
    return RedirectResponse(url="/emp-paylist", status_code=303)


@router.post("/generate-payslip")
async def generate_payslip(request: Request, emp_id: int = Form(...), current_admin: dict = Depends(get_current_admin), db=Depends(get_db)):
    admin_id = current_admin["admin_id"]
    payroll_service.generate_single_payslip(db, emp_id)
    return RedirectResponse(url="/emp-paylist?msg=single", status_code=303)


@router.post("/generate-all")
async def generate_all_payslips(request: Request, current_admin: dict = Depends(get_current_admin), db=Depends(get_db)):
    admin_id = current_admin["admin_id"]
    payroll_service.generate_bulk_payslips(db)
    return RedirectResponse(url="/emp-paylist?msg=bulk", status_code=303)
