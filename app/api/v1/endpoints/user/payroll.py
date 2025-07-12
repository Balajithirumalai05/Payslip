#verify
from datetime import datetime
from PIL import Image, ImageDraw, ImageFont
from reportlab.lib.pagesizes import A4
from reportlab.pdfgen import canvas
import pandas as pd
import os
from fastapi import APIRouter,Request,Depends,HTTPException
from fastapi.responses import HTMLResponse
from app.core.auth import get_current_user
from fastapi.templating import Jinja2Templates
from app.db.session import get_db,SessionLocal
from pathlib import Path
from sqlalchemy.orm import Session
from fastapi.responses import StreamingResponse
from io import BytesIO
from app.models.models import Payslip




router=APIRouter()
templates = Jinja2Templates(directory="app/templates")


@router.get("/payslip", response_class=HTMLResponse)
async def view_payslip(request: Request, current_user: dict = Depends(get_current_user), db: Session = Depends(get_db)):
    
    user_id = current_user['user_id']

    return templates.TemplateResponse("payslip.html", {
        "request": request,
        "user_id": user_id,
    })


@router.get("/payslip/pdf")
def get_payslip_pdf(year: str, month: str, db: Session = Depends(get_db), current_user: dict = Depends(get_current_user)):
    user_id = current_user['user_id']
    print(f"🔍 Fetching payslip for user_id={user_id}, year={year}, month={month}")
    
    payslip = db.query(Payslip).filter_by(user_id=user_id, year=year, month=month).first()
    
    if not payslip:
        print("❌ No payslip record found.")
        raise HTTPException(status_code=404, detail="Payslip not found")
    
    if not payslip.payslip_pdf:
        print("❌ Payslip record found, but no PDF data.")
        raise HTTPException(status_code=404, detail="Payslip data missing")
    
    print(f"✅ Found PDF: {len(payslip.payslip_pdf)} bytes")
    return StreamingResponse(BytesIO(payslip.payslip_pdf), media_type="application/pdf")
