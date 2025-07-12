import app.models.models as models
from fastapi import HTTPException, UploadFile
from sqlalchemy.orm import Session
from app.models import models
from app.core.auth import bcrypt_context

async def create_employee(
    db: Session,
    name: str,
    email: str,
    password: str,
    role: str,
    annual_leave: int,
    special_leave: int,
    sick_leave: int,
    public_leave: int,
    gender: str,
    phone_number: str,
    age: int,
    date_of_birth: str,
    date_of_join: str,
    blood_group: str,
    address: str
):
    # Check if user already exists
    if db.query(models.User).filter(models.User.email == email).first():
        raise HTTPException(status_code=400, detail="User already exists")

    # Create User
    new_user = models.User(email=email, hashed_password=bcrypt_context.hash(password))
    db.add(new_user)

    # Check if employee already exists
    if db.query(models.Employee).filter(models.Employee.name == name).first():
        raise HTTPException(status_code=400, detail="Employee already exists")

    # Create Employee
    new_employee = models.Employee(
        name=name,
        role=role,
        Annual_leave=annual_leave,
        Special_leave=special_leave,
        Sick_leave=sick_leave,
        Public_leave=public_leave
    )

    # Create Profile
    new_profile = models.Profile(
        name=name,
        role=role,
        gender=gender,
        phone_number=phone_number,
        email=email,
        age=age,
        date_of_birth=date_of_birth,
        date_of_join=date_of_join,
        blood_group=blood_group,
        address=address,
        image_url='default.png'
    )

    # Create Payroll
    new_payroll = models.PayRoll(name=name)

    try:
        db.add(new_employee)
        db.add(new_profile)
        db.add(new_payroll)
        db.commit()

        db.refresh(new_user)
        db.refresh(new_employee)
        db.refresh(new_profile)
        db.refresh(new_payroll)

        return {"message": "Employee and profile added successfully"}

    except Exception as e:
        db.rollback()
        raise HTTPException(status_code=500, detail=f"An error occurred: {str(e)}")


def get_admin_dashboard_data(db):
    emp_count = db.query(models.Employee).count()
    leave_req_count = db.query(models.LeaveRequest).filter(models.LeaveRequest.status == "pending").count()
    return emp_count, leave_req_count

def get_employee_name(user_id:int,db:Session):
    employee=db.query(models.Employee).filter(models.Employee.id==user_id).first()
    return employee.name if employee else None

def get_employee_by_id(user_id:int,db:Session):
    employee=db.query(models.Employee).filter(models.Employee.id==user_id).first()
    return employee

def get_profile_by_id(user_id:int,db:Session):
    profile=db.query(models.Profile).filter(models.Profile.id==user_id).first()
    return profile