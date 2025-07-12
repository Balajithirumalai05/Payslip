from sqlalchemy.orm import Session
from app.models.models import Admin
from app.utils.hashing import hash_password


def is_admin_existing(email: str, db: Session) -> bool:
    return db.query(Admin).filter(Admin.email == email).first() is not None


def create_admin(email: str, password: str, db: Session):
    hashed_pw = hash_password(password)
    new_admin = Admin(email=email, hashed_password=hashed_pw)

    db.add(new_admin)
    db.commit()
    db.refresh(new_admin)
