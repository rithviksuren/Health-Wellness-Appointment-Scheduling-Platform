# Non-Profit Donation and Fund Management System

A full-stack web application for non-profit donation, donor, campaign, project funding, receipt, dashboard, and reporting workflows. The app includes a Next.js frontend, a FastAPI backend, Azure-ready infrastructure documentation, and CI/CD scaffolding.

## Features

- Public campaign discovery and donation calls to action
- Donor dashboard, donation history, receipts, profile, settings, and notifications
- Admin dashboard, user management, campaign management, project funding, and reports
- FastAPI backend with SQLAlchemy persistence, Pydantic validation, JWT auth helpers, Redis hooks, and Azure SDK integration points
- Azure Bicep infrastructure scaffold for App Service, SQL, Blob Storage, Redis, Communication Services, Key Vault, Monitor, and Front Door
- GitHub Actions workflows for CI and dev deployment
- Architecture, API, database, deployment, testing, and user documentation

## Tech Stack

- Frontend: Next.js 16, React 19, TypeScript, TailwindCSS
- Backend: Python 3.12, FastAPI, Uvicorn, SQLAlchemy, Pydantic
- Auth and security: JWT helpers, Passlib, bcrypt, RBAC-oriented API design
- Integrations: Azure Blob Storage SDK, Azure Communication Email SDK, Redis client
- Infrastructure: Azure Bicep
- CI/CD: GitHub Actions

## Project Structure

```text
apps/
  web/                 Next.js frontend
  api/                 Original .NET Clean Architecture scaffold
  functions/           Azure Functions scaffold
backend/               FastAPI backend runnable with python main.py
docs/                  Architecture, API, database, deployment, testing docs
infra/bicep/           Azure infrastructure modules
packages/              Shared TypeScript contracts
tests/e2e/             Playwright E2E scaffold
```

## Prerequisites

- Node.js 22+
- npm 11+
- Python 3.12+
- PowerShell on Windows

## Run Locally

Install frontend dependencies from the repository root:

```powershell
npm install
```

Start the backend:

```powershell
cd backend
python -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r requirements.txt
.\.venv\Scripts\python.exe main.py
```

The backend runs at:

```text
http://127.0.0.1:8000
```

Start the frontend from the repository root:

```powershell
npm start
```

The frontend opens automatically at:

```text
http://127.0.0.1:3000
```

## Backend Test Credentials

```text
Email: ava@example.org
Password: Password123!
```

## Useful Commands

```powershell
npm.cmd run web:typecheck
npm.cmd run web:build
python -m py_compile backend\main.py
```

## Environment Variables

The backend works locally with SQLite by default. Optional production-style settings:

- `DATABASE_URL`
- `JWT_SECRET`
- `REDIS_URL`
- `AZURE_STORAGE_CONNECTION_STRING`
- `AZURE_COMMUNICATION_CONNECTION_STRING`
- `BACKEND_HOST`
- `BACKEND_PORT`

## Documentation

- [Architecture](docs/architecture.md)
- [API Documentation](docs/api/api-documentation.md)
- [Database Schema](docs/database-schema.md)
- [Azure Deployment Guide](docs/deployment/azure-deployment-guide.md)
- [Testing Documentation](docs/testing/testing-documentation.md)
- [User Manual](docs/user-manual.md)

## Notes

The FastAPI backend is the local runnable backend. The .NET API and Azure Functions folders are retained as enterprise architecture scaffolding from the original Azure-native plan.
