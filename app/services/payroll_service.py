from fastapi import HTTPException
from fastapi.responses import JSONResponse
import app.models.models as models
from app.utils.payslip_template import generate_payslips
from datetime import datetime
from sqlalchemy import extract
from app.models.models import Attendance


def get_employee_list(db):
    employees = db.query(models.PayRoll).all()
    return [{
        "id": emp.id,
        "name": emp.name,
        "basic": emp.Basic,
        "hra": emp.HRA,
        "other_allowance": emp.Other_Allowance,
        "income_tax": emp.Income_Tax,
        "provident_fund": emp.Provident_Fund
    } for emp in employees]

def get_employee_by_id(db, emp_id):
    emp = db.query(models.PayRoll).filter(models.PayRoll.id == emp_id).first()
    if not emp:
        raise HTTPException(status_code=404, detail="Employee not found")
    return {
        "id": emp.id,
        "name": emp.name,
        "basic": emp.Basic,
        "hra": emp.HRA,
        "other_allowance": emp.Other_Allowance,
        "income_tax": emp.Income_Tax,
        "provident_fund": emp.Provident_Fund
    }

def update_employee_pay_details(db, emp_id, basic, hra, other_allowance, income_tax, provident_fund):
    emp = db.query(models.PayRoll).filter(models.PayRoll.id == emp_id).first()
    if not emp:
        raise HTTPException(status_code=404, detail="Employee not found")
    emp.Basic = basic
    emp.HRA = hra
    emp.Other_Allowance = other_allowance
    emp.Income_Tax = income_tax
    emp.Provident_Fund = provident_fund
    db.commit()
    db.refresh(emp)

def generate_single_payslip(db, emp_id):
    emp = db.query(models.PayRoll).filter(models.PayRoll.id == emp_id).first()
    if not emp:
        raise HTTPException(status_code=404, detail="Employee not found")
    if not all([emp.Basic, emp.HRA, emp.Other_Allowance, emp.Income_Tax, emp.Provident_Fund]):
        raise HTTPException(status_code=400, detail="Please enter complete pay details before generating the payslip.")

    if not generate_payslips(emp, db):
        raise HTTPException(status_code=500, detail="Failed to generate payslip")
    
    return JSONResponse(status_code=200, content={"message": "Payslip generated successfully"})


def generate_bulk_payslips(db):
    employees = db.query(models.PayRoll).all()
    failed = []
    incomplete = []

    for emp in employees:
        if not all([emp.Basic, emp.HRA, emp.Other_Allowance, emp.Income_Tax, emp.Provident_Fund]):
            incomplete.append(emp.name)
            continue

        if not generate_payslips(emp, db):
            failed.append(emp.name)

    if incomplete:
        raise HTTPException(
            status_code=400,
            detail=f"Missing pay details for: {', '.join(incomplete)}. Please update them first."
        )

    if failed:
        raise HTTPException(
            status_code=500,
            detail=f"Failed to generate payslip for: {', '.join(failed)}"
        )

    return JSONResponse(status_code=200, content={"message": "Payslips generated successfully for all complete records."})





