from datetime import datetime
from PIL import Image, ImageDraw, ImageFont
import os
from io import BytesIO
from app.models.models import Payslip,Attendance, PayRoll
from datetime import datetime
from sqlalchemy import extract
from sqlalchemy import func



def generate_payslips(emp, db, template_path="/Users/sudharsanthirumalaivenkatnarayanan/Desktop/Balaji Thirumalai/HR/app/static/Impera_payslip.jpg"):
    current_date = datetime.now().date()
    current_year = current_date.strftime("%Y")
    prev_month = current_date.month - 1 if current_date.month > 1 else 12
    pay_period_date = current_date.replace(day=1, month=prev_month)
    pay_period_month = pay_period_date.strftime("%B")

    try:
        img = Image.open(template_path).convert("RGB")
    except FileNotFoundError:
        print(f"❌ Template image not found at: {template_path}")
        return False

    draw = ImageDraw.Draw(img)

    try:
        font_regular = ImageFont.truetype("app/static/fonts/arial.ttf", 30)
        font_bold = ImageFont.truetype("app/static/fonts/Bebas.ttf", 100)
        font_bottom = ImageFont.truetype("app/static/fonts/Roboto-Bold.ttf", 30)
        for_month = ImageFont.truetype("app/static/fonts/For_month.ttf", 40)
    except OSError:
        print("⚠ Warning: Font file missing! Using default system font.")
        font_regular = font_bold = font_bottom = for_month = ImageFont.load_default()

    positions = {
        "name": (349, 808),
        "employee_id": (349, 873),
        "basic_pay": (150, 1300),
        "hra": (150, 1360),
        "month_year": (1270, 580),
        "other_allowance": (150, 1420),
        "income_tax": (880, 1300),
        "pf": (880, 1360),
        "net_pay": (1845, 1675),
        "total_deductions": (1845, 1575),
        "gross_earnings": (150, 1600),
        "top_net_pay": (1090, 755),
        "paid_days": (1050, 950),
        "LOP_days": (1050, 990),
        "pay_period": (349, 937),
        "pay_date": (349, 1002),
        "Working Hours": (1050, 1030),
        "Amount deduction by hours": (880, 1390)
    }

    amount_right_edge = 818
    amount_right_edge_deductions = 1508

    def draw_right_aligned(draw, label, amount, label_x, label_y, amount_y, font):
        formatted_amount = "₹" + format(amount, ',.2f')
        text_width = draw.textlength(formatted_amount, font=font)
        amount_x = amount_right_edge - text_width
        draw.text((label_x, label_y), label, fill="black", font=font)
        draw.text((amount_x, amount_y), formatted_amount, fill="black", font=font)

    def draw_deduction_right_aligned(draw, label, amount, label_x, label_y, amount_y, font):
        formatted_amount = "₹" + format(amount, ',.2f')
        text_width = draw.textlength(formatted_amount, font=font)
        amount_x = amount_right_edge_deductions - text_width
        draw.text((label_x, label_y), label, fill="black", font=font)
        draw.text((amount_x, amount_y), formatted_amount, fill="black", font=font)

    # ------------------------ CALCULATIONS ------------------------
    # Get fixed salary components
    fixed_basic = emp.Basic
    fixed_hra = emp.HRA
    fixed_allowance = emp.Other_Allowance
    fixed_income_tax = emp.Income_Tax
    fixed_pf = emp.Provident_Fund

    # Attendance-based loss calculation
    today = datetime.now()
    year = today.year
    month = today.month - 1 if today.month > 1 else 12

    total_hours = db.query(func.sum(Attendance.hours_worked)).filter(
        Attendance.user_id == emp.id,
        extract('month', Attendance.login_time) == month,
        extract('year', Attendance.login_time) == year
    ).scalar() or 0

    per_hour = fixed_basic / 160
    worked_hours = min(total_hours, 160)
    lost_hours = 160 - worked_hours
    basic_deduction = round(per_hour * lost_hours, 2)

    # Gross and Net
    gross_earnings = fixed_basic + fixed_hra + fixed_allowance
    total_deductions = fixed_income_tax + fixed_pf + basic_deduction
    net_pay = gross_earnings - total_deductions

    # ------------------------ DRAWING ON TEMPLATE ------------------------
    draw.text(positions["name"], f" :  {emp.name}", fill="black", font=font_regular)
    draw.text(positions["employee_id"], f" :{str(emp.id):>4}", fill="black", font=font_regular)

    draw_right_aligned(draw, "Basic Pay", fixed_basic, 150, 1300, 1300, font_regular)
    draw_right_aligned(draw, "HRA", fixed_hra, 150, 1360, 1360, font_regular)
    draw_right_aligned(draw, "Other Allowance", fixed_allowance, 150, 1420, 1420, font_regular)

    draw_deduction_right_aligned(draw, "Income Tax:", fixed_income_tax, 880, 1300, 1300, font_regular)
    draw_deduction_right_aligned(draw, "Provident Fund:", fixed_pf, 880, 1360, 1360, font_regular)

    draw.text(positions["Working Hours"], f"Working Hours:  {worked_hours}", fill="black", font=font_regular)
    draw_deduction_right_aligned(
        draw,
        "Deduction (Hours):",
        basic_deduction,
        880, 1420, 1420,  # label_x, label_y, amount_y
        font_regular
    )

    draw.text(positions["paid_days"], f"Paid days: 30 ", fill="black", font=font_regular)
    draw.text(positions["LOP_days"], f"LOP days: 0", fill="black", font=font_regular)
    draw.text(positions["month_year"], f" {pay_period_month}, {current_year}", fill="#696969", font=for_month)
    draw.text(positions["pay_period"], f" : {pay_period_month}, {current_year}", fill="black", font=font_regular)
    draw.text(positions["pay_date"], f" : {current_date.strftime('%d-%m-%Y')}", fill="black", font=font_regular)

    draw_right_aligned(draw, "Gross Earnings", gross_earnings, 220, 1603, 1603, font_bottom)
    draw_deduction_right_aligned(draw, "Total Deductions:", total_deductions, 880, 1603, 1603, font_bottom)
    draw_deduction_right_aligned(draw, "", net_pay, 880, 1740, 1740, font_bottom)
    draw.text(positions["top_net_pay"], f"{'₹' + format(net_pay, ',.0f')}", fill="black", font=font_bold)

    # ------------------------ SAVE TO DATABASE ------------------------
    try:
        buffer = BytesIO()
        img.save(buffer, "PDF")
        buffer.seek(0)
        pdf_data = buffer.read()

        existing = db.query(Payslip).filter_by(user_id=emp.id, month=pay_period_month, year=current_year).first()
        if existing:
            existing.payslip_pdf = pdf_data
        else:
            new_payslip = Payslip(
                user_id=emp.id,
                month=pay_period_month,
                year=current_year,
                payslip_pdf=pdf_data
            )
            db.add(new_payslip)

        db.commit()
        return True
    except Exception as e:
        print(f"❌ Error saving payslip: {e}")
        return False
