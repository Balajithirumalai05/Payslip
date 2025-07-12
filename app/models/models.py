from datetime import datetime
from sqlalchemy import Column,Integer,String,Date,Float,ForeignKey,Time,DateTime,LargeBinary
from sqlalchemy.orm import relationship
from app.db.session import Base
from sqlalchemy.dialects.mysql import LONGBLOB  
from sqlalchemy import Column, Integer, Float, DateTime, ForeignKey
from sqlalchemy.orm import relationship
from datetime import datetime

class User(Base):
    __tablename__ ="user"
    id=Column(Integer,primary_key=True,index=True)
    email=Column(String(100),unique=True,nullable=False)
    hashed_password=Column(String(100),nullable=False)
    reset_token=Column(String(100))
    attendances = relationship("Attendance", back_populates="user", cascade="all, delete")



class Employee(Base):
    __tablename__='employees'

    id=Column(Integer,primary_key=True)
    name=Column(String(100),nullable=False)
    role=Column(String(100),nullable=False)
    Annual_leave=Column(Integer,nullable=True)
    Special_leave=Column(Integer,nullable=True)
    Sick_leave=Column(Integer,nullable=True)
    Public_leave=Column(Integer,nullable=True)



    leave_requests=relationship("LeaveRequest",back_populates="employee")
    buddy_requests = relationship("BuddyRequest", back_populates="employee", cascade="all, delete-orphan")
    payroll=relationship("PayRoll",back_populates="employee")
    profile=relationship("Profile",back_populates="employee")


class LeaveRequest(Base):
    __tablename__='leave_requests'

    id=Column(Integer,primary_key=True)
    employee_id=Column(Integer,ForeignKey('employees.id'))
    leave_type=Column(String(100),nullable=False)
    start_date=Column(Date,nullable=False)
    end_date=Column(Date,nullable=False)
    request_date = Column(DateTime, default=datetime.now()) 
    leave_days=Column(Integer)
    status=Column(String(20))
    employee=relationship(Employee,back_populates="leave_requests")

class Timesheet(Base):
    __tablename__ = "timesheets"

    id = Column(Integer, primary_key=True, index=True)
    employee_id = Column(Integer, ForeignKey("employees.id"))
    date = Column(Date)
    mode = Column(String(50))
    location = Column(String(50))
    hours = Column(Float)
    billable = Column(String(50))
    project_name = Column(String(100))
    client_name = Column(String(50))
    notes = Column(String(255))
    category = Column(String(20))  # "External", "Internal", "Absence"

    status = Column(String(20), default="Pending")
    admin_comment = Column(String(255), nullable=True)
    submitted_on = Column(DateTime, default=datetime.utcnow)
    reviewed_on = Column(DateTime, nullable=True)

    # 👇 THIS LINE IS IMPORTANT
    employee = relationship("Employee", backref="timesheets")

class Profile(Base):
    __tablename__ = 'profiles'

    id = Column(Integer, primary_key=True, index=True)
    employee_id = Column(Integer, ForeignKey('employees.id'))
    role = Column(String(100), nullable=False)
    name = Column(String(100), nullable=False)
    gender = Column(String(10), nullable=False)
    phone_number=Column(String(20),nullable=True)
    email=Column(String(50),nullable=True)
    age=Column(Integer,nullable=False)
    date_of_birth = Column(Date, nullable=False)
    date_of_join = Column(Date, nullable=False)
    blood_group = Column(String(5), nullable=True) 
    address = Column(String(255), nullable=True)
    image_url = Column(String(255))
    manager = Column(String(100))
    emergency_contact_name = Column(String(100))
    emergency_contact_relation = Column(String(100))
    emergency_contact_number = Column(String(15))

    employee = relationship(Employee, back_populates="profile")



class PayRoll(Base):
    __tablename__='payroll'
    id=Column(Integer,primary_key=True,index=True)
    employee_id=Column(Integer,ForeignKey('employees.id'))
    name=Column(String(100),nullable=False)
    Basic=Column(Float)
    HRA=Column(Float)
    Other_Allowance=Column(Float)
    Income_Tax=Column(Float)
    Provident_Fund=Column(Float)
    Payslip_Path=Column(String(100))
    Paid_days=Column(Integer)
    LOP_days=Column(Integer)
    employee=relationship(Employee,back_populates="payroll")

class Holiday(Base):
    __tablename__ = 'holidays'
    id = Column(Integer, primary_key=True, index=True)
    name = Column(String(100), nullable=False)
    date = Column(Date, nullable=False)
    day=Column(String(50),nullable=False)

class BuddyRequest(Base):
    __tablename__ = "buddy_requests"

    id = Column(Integer, primary_key=True, index=True)
    employee_id = Column(Integer, ForeignKey("employees.id"), nullable=False)
    buddy_name = Column(String(100), nullable=False)
    buddy_relation = Column(String(100), nullable=False)
    buddy_profession = Column(String(100), nullable=False)
    buddy_mobile_no = Column(String(15), nullable=False)
    buddy_email = Column(String(100), nullable=False)
    buddy_documents = Column(LargeBinary, nullable=True)  # For storing uploaded documents

    employee = relationship("Employee", back_populates="buddy_requests")

class Admin(Base):
    __tablename__ = "admins"

    id = Column(Integer, primary_key=True, index=True)
    email = Column(String(40), unique=True, index=True)
    hashed_password = Column(String(255))

class Payslip(Base):
    __tablename__ = 'payslips'

    id = Column(Integer, primary_key=True)
    user_id = Column(Integer)
    month = Column(String(255))
    year = Column(String(255))
    payslip_pdf = Column(LONGBLOB)


class Attendance(Base):
    __tablename__ = "attendance"

    id = Column(Integer, primary_key=True)
    user_id = Column(Integer, ForeignKey("user.id"))
    login_time = Column(DateTime, default=datetime.utcnow)
    logout_time = Column(DateTime, nullable=True)
    hours_worked = Column(Float, default=0.0)

    user = relationship("User", back_populates="attendances")
