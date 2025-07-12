from datetime import timedelta, datetime
from typing import Annotated
from fastapi import APIRouter, Depends, HTTPException, Cookie, Request
from pydantic import BaseModel
from sqlalchemy.orm import Session
from starlette import status
from app.db.session import SessionLocal, get_db
from app.models.models import User,Admin
from passlib.context import CryptContext
from fastapi.security import OAuth2PasswordBearer,OAuth2PasswordRequestForm
from jose import jwt, JWTError
from fastapi.responses import RedirectResponse
import os
from dotenv import load_dotenv

load_dotenv()



router = APIRouter(prefix='/auth', tags=['auth'])

SECRET_KEY="kafhaijsfasdasd"
ALGORITHM="HS256"

bcrypt_context=CryptContext(schemes=['bcrypt'],deprecated='auto')
async def oauth2_bearer_from_cookie(access_token: str = Cookie(None)):
    if not access_token:
        raise HTTPException(status_code=401, detail="Not authenticated")
    return access_token

async def oauth2_bearer_from_cookie_a(admin_access_token: str = Cookie(None)):
    if not admin_access_token:
        raise HTTPException(status_code=401, detail="Not authenticated")
    return admin_access_token



class CreateUserRequest(BaseModel):
    username:str
    password:str
class Token(BaseModel):
    access_token:str
    token_type:str

db_dependency=Annotated[Session,Depends(get_db)]



@router.post("/",status_code=status.HTTP_201_CREATED)
async def create_user(db:db_dependency,create_user_request:CreateUserRequest):
    new_user=User(username=create_user_request.username,hashed_password=bcrypt_context.hash(create_user_request.password),)
    db.add(new_user)
    db.commit()

@router.post("/token",response_model=Token)
async def login_access_token(form_data:Annotated[OAuth2PasswordRequestForm,Depends()],db:db_dependency):
    user=authenticate_user(form_data.username,form_data.password,db)
    if not user:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED,detail='Could not validate user')
    token=create_access_token(user.email,user.id,timedelta(minutes=30))
    return {'access_token':token,'token_type':'bearer'}

def authenticate_user(username:str,password:str,db):
    user=db.query(User).filter(User.email==username).first()
    if not user:
        return False
    if not bcrypt_context.verify(password,user.hashed_password):
        return False
    return user

def authenticate_admin(email: str, password: str, db):
    admin = db.query(Admin).filter(Admin.email == email).first()
    if not admin:  # Check admin variable, not Admin class
        return False
    if not bcrypt_context.verify(password, admin.hashed_password):
        return False
    return admin

def create_access_token(username: str, user_id: int, expires_delta: timedelta):
    encode = {'sub': username, 'id': user_id}
    expires = datetime.utcnow() + expires_delta
    encode.update({'exp': expires})
    return jwt.encode(encode, SECRET_KEY, algorithm=ALGORITHM)

async def get_current_user(token: str = Depends(oauth2_bearer_from_cookie)):
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        username: str = payload.get('sub')
        user_id: int = payload.get('id')
        if username is None or user_id is None:
            raise HTTPException(status_code=401, detail="Invalid token payload")
        return {"username": username, "user_id": user_id}
    except JWTError:
        raise HTTPException(status_code=401, detail="Could not validate credentials")

async def get_current_admin(token: str = Depends(oauth2_bearer_from_cookie_a)):
    # token = request.cookies.get("admin_access_token")
    # if not token:
    #     raise HTTPException(status_code=401, detail="Not authenticated")
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        email: str = payload.get('sub')
        admin_id: int = payload.get('id')
        if email is None or admin_id is None:
            raise HTTPException(status_code=401, detail="Invalid token payload")
        return {"adminname": email, "admin_id": admin_id}
        # admin = db.query(Admin).filter(Admin.id == admin_id).first()
        # if not admin:
        #     raise HTTPException(status_code=401, detail="Admin not found")
        # return admin
    except JWTError as e:
        raise HTTPException(status_code=401, detail="Could not validate credentials")

@router.post("/logout")
async def logout():
    response = RedirectResponse(url="/", status_code=status.HTTP_302_FOUND)
    response.delete_cookie("access_token")
    return response

@router.post("/token/admin", response_model=Token)
async def admin_login_access_token(
    form_data: Annotated[OAuth2PasswordRequestForm, Depends()], 
    db: db_dependency
):
    admin = authenticate_admin(form_data.username, form_data.password, db)  # Use username
    if not admin:
        raise HTTPException(status_code=401, detail='Invalid credentials')
    admin_token = create_access_token(admin.email, admin.id, timedelta(minutes=30))
    return {'admin_access_token': admin_token, 'token_type': 'bearer'}

@router.post("/admin/logout")
async def logout():
    response = RedirectResponse(url="/admin/login", status_code=status.HTTP_302_FOUND)
    response.delete_cookie("admin_access_token")
    return response