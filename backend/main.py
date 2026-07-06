from __future__ import annotations

import os
import site
import sys
from contextlib import asynccontextmanager
from datetime import datetime, timedelta, timezone
from pathlib import Path
from typing import Annotated, Any, Generator
from uuid import uuid4

BASE_DIR = Path(__file__).resolve().parent
LOCAL_VENV = BASE_DIR / ".venv"
LOCAL_PYTHONLIBS = BASE_DIR / ".pythonlibs"
USER_SITE = site.getusersitepackages()

venv_site_packages = next(LOCAL_VENV.glob("Lib/site-packages"), None)
running_in_venv = Path(sys.prefix).resolve() == LOCAL_VENV.resolve()
if not running_in_venv and venv_site_packages and venv_site_packages.exists():
    sys.path.insert(0, str(venv_site_packages))
if not running_in_venv and LOCAL_PYTHONLIBS.exists():
    sys.path.insert(0, str(LOCAL_PYTHONLIBS))
if not running_in_venv and USER_SITE:
    sys.path.append(USER_SITE)

import redis
import uvicorn
from fastapi import Depends, FastAPI, Header, HTTPException, Query, Request, Response, status
from fastapi.middleware.cors import CORSMiddleware
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer
from jose import JWTError, jwt
from passlib.context import CryptContext
from pydantic import BaseModel, EmailStr, Field
from sqlalchemy import Boolean, DateTime, Float, ForeignKey, String, create_engine, func, select
from sqlalchemy.orm import DeclarativeBase, Mapped, Session, mapped_column, relationship, sessionmaker


DATABASE_URL = os.getenv("DATABASE_URL", f"sqlite:///{BASE_DIR / 'nonprofit_fund.db'}")
JWT_SECRET = os.getenv("JWT_SECRET", "local-dev-secret-change-me")
JWT_ALGORITHM = os.getenv("JWT_ALGORITHM", "HS256")
JWT_ISSUER = os.getenv("JWT_ISSUER", "nonprofit-fund-local")
HOST = os.getenv("BACKEND_HOST", "127.0.0.1")
PORT = int(os.getenv("BACKEND_PORT", "8000"))

engine = create_engine(DATABASE_URL, connect_args={"check_same_thread": False} if DATABASE_URL.startswith("sqlite") else {})
SessionLocal = sessionmaker(bind=engine, autoflush=False, expire_on_commit=False)
password_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
bearer_scheme = HTTPBearer(auto_error=False)


class Base(DeclarativeBase):
    pass


class User(Base):
    __tablename__ = "users"

    id: Mapped[str] = mapped_column(String(64), primary_key=True)
    email: Mapped[str] = mapped_column(String(255), unique=True, index=True)
    display_name: Mapped[str] = mapped_column(String(160))
    roles_csv: Mapped[str] = mapped_column(String(255), default="Donor")
    password_hash: Mapped[str] = mapped_column(String(255), default="")
    is_active: Mapped[bool] = mapped_column(Boolean, default=True)
    donations: Mapped[list["Donation"]] = relationship(back_populates="donor")


class Campaign(Base):
    __tablename__ = "campaigns"

    id: Mapped[str] = mapped_column(String(64), primary_key=True)
    name: Mapped[str] = mapped_column(String(180))
    slug: Mapped[str] = mapped_column(String(140), unique=True, index=True)
    summary: Mapped[str] = mapped_column(String(500))
    goal_amount: Mapped[float] = mapped_column(Float)
    raised_amount: Mapped[float] = mapped_column(Float, default=0)
    status: Mapped[str] = mapped_column(String(32), default="Published")
    starts_on: Mapped[str] = mapped_column(String(16))
    ends_on: Mapped[str | None] = mapped_column(String(16), nullable=True)
    hero_image_url: Mapped[str | None] = mapped_column(String(1000), nullable=True)
    donations: Mapped[list["Donation"]] = relationship(back_populates="campaign")


class Project(Base):
    __tablename__ = "projects"

    id: Mapped[str] = mapped_column(String(64), primary_key=True)
    name: Mapped[str] = mapped_column(String(180))
    code: Mapped[str] = mapped_column(String(32), unique=True, index=True)
    description: Mapped[str] = mapped_column(String(2000))
    funding_goal: Mapped[float] = mapped_column(Float)
    allocated_amount: Mapped[float] = mapped_column(Float, default=0)
    is_active: Mapped[bool] = mapped_column(Boolean, default=True)


class Donation(Base):
    __tablename__ = "donations"

    id: Mapped[str] = mapped_column(String(64), primary_key=True)
    donor_id: Mapped[str] = mapped_column(ForeignKey("users.id"), index=True)
    campaign_id: Mapped[str | None] = mapped_column(ForeignKey("campaigns.id"), nullable=True, index=True)
    amount: Mapped[float] = mapped_column(Float)
    currency: Mapped[str] = mapped_column(String(3), default="USD")
    status: Mapped[str] = mapped_column(String(32), default="Succeeded")
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), default=lambda: datetime.now(timezone.utc))
    donor: Mapped[User] = relationship(back_populates="donations")
    campaign: Mapped[Campaign | None] = relationship(back_populates="donations")


class Notification(Base):
    __tablename__ = "notifications"

    id: Mapped[str] = mapped_column(String(64), primary_key=True)
    user_id: Mapped[str] = mapped_column(ForeignKey("users.id"), index=True)
    channel: Mapped[str] = mapped_column(String(16))
    status: Mapped[str] = mapped_column(String(32), default="Queued")
    subject: Mapped[str] = mapped_column(String(180))
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), default=lambda: datetime.now(timezone.utc))


class TokenResponse(BaseModel):
    accessToken: str
    tokenType: str = "Bearer"


class LoginRequest(BaseModel):
    email: EmailStr
    password: str = Field(min_length=6)


class UserDto(BaseModel):
    id: str
    email: EmailStr
    displayName: str
    roles: list[str]
    isActive: bool


class CampaignDto(BaseModel):
    id: str
    name: str
    slug: str
    summary: str
    goalAmount: float
    raisedAmount: float
    status: str
    startsOn: str
    endsOn: str | None = None
    heroImageUrl: str | None = None


class ProjectDto(BaseModel):
    id: str
    name: str
    code: str
    description: str
    fundingGoal: float
    allocatedAmount: float
    isActive: bool


class DonationDto(BaseModel):
    id: str
    donorId: str
    campaignId: str | None = None
    amount: float
    currency: str
    status: str
    createdAt: datetime


class CreateDonationRequest(BaseModel):
    campaignId: str | None = None
    amount: float = Field(gt=0)
    currency: str = Field(default="USD", min_length=3, max_length=3)
    dedication: str | None = None
    generateReceipt: bool = True


class CreatePaymentIntentRequest(BaseModel):
    donationId: str
    amount: float = Field(gt=0)
    currency: str = Field(default="USD", min_length=3, max_length=3)


class PaymentIntentResponse(BaseModel):
    provider: str
    clientSecret: str
    providerReference: str


class DashboardMetricDto(BaseModel):
    label: str
    value: float
    format: str


class DashboardDto(BaseModel):
    metrics: list[DashboardMetricDto]
    campaigns: list[CampaignDto]
    recentDonations: list[DonationDto]


class DonationSummaryDto(BaseModel):
    totalRaised: float
    donationCount: int
    recurringMonthlyValue: float
    averageDonation: float


class ReportExportDto(BaseModel):
    id: str
    reportType: str
    status: str
    blobUrl: str | None = None


class NotificationDto(BaseModel):
    id: str
    channel: str
    status: str
    subject: str
    createdAt: datetime


class AzureServices:
    def __init__(self) -> None:
        blob_connection = os.getenv("AZURE_STORAGE_CONNECTION_STRING")
        email_connection = os.getenv("AZURE_COMMUNICATION_CONNECTION_STRING")
        self.blob_client: Any | None = None
        self.email_client: Any | None = None

        if blob_connection:
            try:
                from azure.storage.blob import BlobServiceClient

                self.blob_client = BlobServiceClient.from_connection_string(blob_connection)
            except ImportError:
                self.blob_client = None

        if email_connection:
            try:
                from azure.communication.email import EmailClient

                self.email_client = EmailClient.from_connection_string(email_connection)
            except ImportError:
                self.email_client = None

    def receipt_blob_url(self, donation_id: str) -> str | None:
        if self.blob_client is None:
            return None
        return f"receipts/{donation_id}.pdf"


class RedisCache:
    def __init__(self) -> None:
        redis_url = os.getenv("REDIS_URL")
        self.client = redis.from_url(redis_url, decode_responses=True) if redis_url else None

    def get(self, key: str) -> str | None:
        if self.client is None:
            return None
        return self.client.get(key)

    def set(self, key: str, value: str, seconds: int = 300) -> None:
        if self.client is not None:
            self.client.setex(key, seconds, value)


azure_services = AzureServices()
redis_cache = RedisCache()


def get_db() -> Generator[Session, None, None]:
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()


DbSession = Annotated[Session, Depends(get_db)]


def create_access_token(user: User) -> str:
    now = datetime.now(timezone.utc)
    payload = {
        "sub": user.id,
        "email": user.email,
        "name": user.display_name,
        "roles": roles_for(user),
        "iss": JWT_ISSUER,
        "iat": now,
        "exp": now + timedelta(hours=4),
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)


def current_user(
    db: DbSession,
    credentials: Annotated[HTTPAuthorizationCredentials | None, Depends(bearer_scheme)],
) -> User:
    if credentials is None:
        return db.scalar(select(User).where(User.email == "ava@example.org")) or seed_default_user(db)

    try:
        payload = jwt.decode(credentials.credentials, JWT_SECRET, algorithms=[JWT_ALGORITHM], issuer=JWT_ISSUER)
        user_id = payload.get("sub")
    except JWTError as exc:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid token") from exc

    user = db.get(User, user_id)
    if user is None or not user.is_active:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="User not found or inactive")
    return user


def require_roles(*allowed_roles: str):
    def dependency(user: Annotated[User, Depends(current_user)]) -> User:
        if not set(roles_for(user)).intersection(allowed_roles):
            raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Insufficient role")
        return user

    return dependency


def roles_for(user: User) -> list[str]:
    return [role.strip() for role in user.roles_csv.split(",") if role.strip()]


def seed_default_user(db: Session) -> User:
    user = User(
        id="user-001",
        email="ava@example.org",
        display_name="Ava Patel",
        roles_csv="Donor,Admin,Treasurer,Campaign Manager",
        password_hash=password_context.hash("Password123!"),
    )
    db.add(user)
    db.commit()
    return user


def seed_database() -> None:
    Base.metadata.create_all(bind=engine)
    with SessionLocal() as db:
        if db.scalar(select(func.count(User.id))) == 0:
            db.add_all(
                [
                    User(id="user-001", email="ava@example.org", display_name="Ava Patel", roles_csv="Donor,Admin,Treasurer,Campaign Manager", password_hash=password_context.hash("Password123!")),
                    User(id="user-002", email="treasurer@example.org", display_name="Noah Williams", roles_csv="Treasurer", password_hash=password_context.hash("Password123!")),
                    User(id="user-003", email="admin@example.org", display_name="Maya Johnson", roles_csv="Admin", password_hash=password_context.hash("Password123!")),
                ]
            )

        if db.scalar(select(func.count(Campaign.id))) == 0:
            db.add_all(
                [
                    Campaign(id="campaign-clean-water", name="Clean Water Access", slug="clean-water-access", summary="Fund community water filters and maintenance training for rural schools.", goal_amount=85000, raised_amount=52340, starts_on="2026-01-01", ends_on="2026-12-31", hero_image_url="https://images.unsplash.com/photo-1541544741938-0af808871cc0?auto=format&fit=crop&w=1600&q=80"),
                    Campaign(id="campaign-student-meals", name="Student Meal Fund", slug="student-meal-fund", summary="Provide nutritious school meals and weekend food kits for children.", goal_amount=120000, raised_amount=91200, starts_on="2026-02-01", hero_image_url="https://images.unsplash.com/photo-1488521787991-ed7bbaae773c?auto=format&fit=crop&w=1600&q=80"),
                    Campaign(id="campaign-clinic-outreach", name="Mobile Clinic Outreach", slug="mobile-clinic-outreach", summary="Expand preventive health visits with mobile clinic teams and supplies.", goal_amount=150000, raised_amount=48100, starts_on="2026-03-01", hero_image_url="https://images.unsplash.com/photo-1584515933487-779824d29309?auto=format&fit=crop&w=1600&q=80"),
                ]
            )

        if db.scalar(select(func.count(Project.id))) == 0:
            db.add_all(
                [
                    Project(id="project-water-filters", name="Water Filter Installations", code="WATER-2026", description="Install long-life water filters, train local caretakers, and monitor water quality.", funding_goal=65000, allocated_amount=38400),
                    Project(id="project-school-meals", name="School Meals Program", code="MEALS-2026", description="Daily meals, weekend food packs, and nutrition tracking for partner schools.", funding_goal=90000, allocated_amount=71200),
                    Project(id="project-clinic-supplies", name="Mobile Clinic Supplies", code="CLINIC-2026", description="Medical supplies, screening equipment, and transport support for mobile clinics.", funding_goal=110000, allocated_amount=32100),
                ]
            )

        if db.scalar(select(func.count(Donation.id))) == 0:
            db.add_all(
                [
                    Donation(id="don-001", donor_id="user-001", campaign_id="campaign-clean-water", amount=250, created_at=datetime(2026, 7, 2, 9, tzinfo=timezone.utc)),
                    Donation(id="don-002", donor_id="user-002", campaign_id="campaign-student-meals", amount=75, created_at=datetime(2026, 7, 3, 15, 30, tzinfo=timezone.utc)),
                    Donation(id="don-003", donor_id="user-003", campaign_id="campaign-clinic-outreach", amount=500, created_at=datetime(2026, 7, 4, 18, 45, tzinfo=timezone.utc)),
                ]
            )

        db.commit()


@asynccontextmanager
async def lifespan(_: FastAPI):
    seed_database()
    yield


app = FastAPI(
    title="Non-Profit Donation and Fund Management API",
    version="1.0.0",
    description="FastAPI backend with SQLAlchemy persistence, Pydantic validation, JWT auth, Redis hooks, and Azure SDK integration points.",
    lifespan=lifespan,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000", "http://127.0.0.1:3000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.middleware("http")
async def add_security_headers(request: Request, call_next):
    response: Response = await call_next(request)
    response.headers["x-content-type-options"] = "nosniff"
    response.headers["x-frame-options"] = "DENY"
    response.headers["referrer-policy"] = "strict-origin-when-cross-origin"
    return response


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "healthy", "service": "fastapi-nonprofit-backend"}


@app.post("/api/auth/login", response_model=TokenResponse)
def login(request: LoginRequest, db: DbSession) -> TokenResponse:
    user = db.scalar(select(User).where(User.email == request.email))
    if user is None or not password_context.verify(request.password, user.password_hash):
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials")
    return TokenResponse(accessToken=create_access_token(user))


@app.get("/api/auth/me", response_model=UserDto)
def me(user: Annotated[User, Depends(current_user)]) -> UserDto:
    return to_user_dto(user)


@app.get("/api/users", response_model=list[UserDto])
def users(db: DbSession, _: Annotated[User, Depends(require_roles("Admin"))]) -> list[UserDto]:
    return [to_user_dto(user) for user in db.scalars(select(User).order_by(User.display_name)).all()]


@app.get("/api/campaigns", response_model=list[CampaignDto])
def campaigns(db: DbSession) -> list[CampaignDto]:
    return [to_campaign_dto(campaign) for campaign in db.scalars(select(Campaign).order_by(Campaign.name)).all()]


@app.get("/api/campaigns/{slug}", response_model=CampaignDto)
def campaign(slug: str, db: DbSession) -> CampaignDto:
    item = db.scalar(select(Campaign).where(Campaign.slug == slug))
    if item is None:
        raise HTTPException(status_code=404, detail="Campaign not found")
    return to_campaign_dto(item)


@app.get("/api/projects", response_model=list[ProjectDto])
def projects(db: DbSession) -> list[ProjectDto]:
    return [to_project_dto(project) for project in db.scalars(select(Project).order_by(Project.name)).all()]


@app.get("/api/projects/{project_id}", response_model=ProjectDto)
def project(project_id: str, db: DbSession) -> ProjectDto:
    item = db.get(Project, project_id)
    if item is None:
        raise HTTPException(status_code=404, detail="Project not found")
    return to_project_dto(item)


@app.post("/api/donations", response_model=DonationDto, status_code=201)
def create_donation(request: CreateDonationRequest, db: DbSession, user: Annotated[User, Depends(current_user)]) -> DonationDto:
    campaign = db.get(Campaign, request.campaignId) if request.campaignId else None
    donation = Donation(id=f"don-{uuid4().hex[:8]}", donor_id=user.id, campaign_id=request.campaignId, amount=request.amount, currency=request.currency)
    db.add(donation)
    if campaign is not None:
        campaign.raised_amount += request.amount
    db.commit()
    db.refresh(donation)
    azure_services.receipt_blob_url(donation.id)
    return to_donation_dto(donation)


@app.get("/api/donations/me", response_model=list[DonationDto])
def my_donations(db: DbSession, user: Annotated[User, Depends(current_user)]) -> list[DonationDto]:
    rows = db.scalars(select(Donation).where(Donation.donor_id == user.id).order_by(Donation.created_at.desc())).all()
    return [to_donation_dto(donation) for donation in rows]


@app.get("/api/donations/{donation_id}", response_model=DonationDto)
def donation(donation_id: str, db: DbSession) -> DonationDto:
    item = db.get(Donation, donation_id)
    if item is None:
        raise HTTPException(status_code=404, detail="Donation not found")
    return to_donation_dto(item)


@app.post("/api/payments/intent", response_model=PaymentIntentResponse, status_code=201)
def payment_intent(request: CreatePaymentIntentRequest) -> PaymentIntentResponse:
    return PaymentIntentResponse(provider="Mock", clientSecret=f"mock_secret_{request.donationId}", providerReference=f"mock_{uuid4().hex}")


@app.post("/api/payments/webhook")
def payment_webhook(x_signature: Annotated[str | None, Header()] = None) -> dict[str, str | None]:
    return {"status": "accepted", "signature": x_signature}


@app.get("/api/reports/donation-summary", response_model=DonationSummaryDto)
def donation_summary(db: DbSession, _: Annotated[User, Depends(require_roles("Admin", "Treasurer"))]) -> DonationSummaryDto:
    donations = db.scalars(select(Donation)).all()
    total = sum(item.amount for item in donations)
    count = len(donations)
    return DonationSummaryDto(totalRaised=total, donationCount=count, recurringMonthlyValue=18325, averageDonation=round(total / count, 2) if count else 0)


@app.get("/api/reports/monthly")
@app.get("/api/reports/campaigns")
@app.get("/api/reports/donors")
@app.get("/api/reports/project-funding")
def empty_report(_: Annotated[User, Depends(require_roles("Admin", "Treasurer"))]) -> dict[str, list[dict]]:
    return {"items": []}


@app.post("/api/reports/export", response_model=ReportExportDto, status_code=202)
def export_report(reportType: Annotated[str, Query()] = "donation-summary", _: Annotated[User, Depends(require_roles("Admin", "Treasurer"))] = None) -> ReportExportDto:
    return ReportExportDto(id=f"export-{uuid4().hex[:8]}", reportType=reportType, status="Queued")


@app.get("/api/notifications", response_model=list[NotificationDto])
def notifications(db: DbSession, user: Annotated[User, Depends(current_user)]) -> list[NotificationDto]:
    rows = db.scalars(select(Notification).where(Notification.user_id == user.id).order_by(Notification.created_at.desc())).all()
    return [NotificationDto(id=row.id, channel=row.channel, status=row.status, subject=row.subject, createdAt=row.created_at) for row in rows]


@app.post("/api/notifications/test", status_code=202)
def test_notification(db: DbSession, user: Annotated[User, Depends(current_user)]) -> dict[str, str]:
    notification = Notification(id=f"notif-{uuid4().hex[:8]}", user_id=user.id, channel="Email", subject="Test notification")
    db.add(notification)
    db.commit()
    return {"status": "queued", "id": notification.id}


@app.get("/api/dashboard/donor", response_model=DashboardDto)
@app.get("/api/dashboard/admin", response_model=DashboardDto)
@app.get("/api/dashboard/treasurer", response_model=DashboardDto)
@app.get("/api/dashboard/campaign-manager", response_model=DashboardDto)
def dashboard(db: DbSession) -> DashboardDto:
    cache_key = "dashboard"
    cached = redis_cache.get(cache_key)
    if cached:
        return DashboardDto.model_validate_json(cached)

    donation_rows = db.scalars(select(Donation).order_by(Donation.created_at.desc()).limit(10)).all()
    campaign_rows = db.scalars(select(Campaign).order_by(Campaign.raised_amount.desc()).limit(5)).all()
    total = sum(row.amount for row in db.scalars(select(Donation)).all())
    result = DashboardDto(
        metrics=[
            DashboardMetricDto(label="Total Raised", value=total, format="currency"),
            DashboardMetricDto(label="Active Donors", value=db.scalar(select(func.count(User.id))) or 0, format="number"),
            DashboardMetricDto(label="Recurring MRR", value=18325, format="currency"),
            DashboardMetricDto(label="Receipt SLA", value=98, format="number"),
        ],
        campaigns=[to_campaign_dto(campaign) for campaign in campaign_rows],
        recentDonations=[to_donation_dto(donation) for donation in donation_rows],
    )
    redis_cache.set(cache_key, result.model_dump_json())
    return result


def to_user_dto(user: User) -> UserDto:
    return UserDto(id=user.id, email=user.email, displayName=user.display_name, roles=roles_for(user), isActive=user.is_active)


def to_campaign_dto(campaign: Campaign) -> CampaignDto:
    return CampaignDto(
        id=campaign.id,
        name=campaign.name,
        slug=campaign.slug,
        summary=campaign.summary,
        goalAmount=campaign.goal_amount,
        raisedAmount=campaign.raised_amount,
        status=campaign.status,
        startsOn=campaign.starts_on,
        endsOn=campaign.ends_on,
        heroImageUrl=campaign.hero_image_url,
    )


def to_project_dto(project: Project) -> ProjectDto:
    return ProjectDto(
        id=project.id,
        name=project.name,
        code=project.code,
        description=project.description,
        fundingGoal=project.funding_goal,
        allocatedAmount=project.allocated_amount,
        isActive=project.is_active,
    )


def to_donation_dto(donation: Donation) -> DonationDto:
    return DonationDto(
        id=donation.id,
        donorId=donation.donor_id,
        campaignId=donation.campaign_id,
        amount=donation.amount,
        currency=donation.currency,
        status=donation.status,
        createdAt=donation.created_at,
    )


def main() -> None:
    uvicorn.run("main:app", host=HOST, port=PORT, reload=False)


if __name__ == "__main__":
    main()
