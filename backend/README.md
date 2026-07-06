# FastAPI Backend

Run from this directory:

```powershell
python -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r requirements.txt
.\.venv\Scripts\python.exe main.py
```

If you do not want a virtual environment, this fallback also works:

```powershell
python -m pip install -r requirements.txt --target .pythonlibs --upgrade
python main.py
```

The API listens on `http://127.0.0.1:8000`.

The API uses FastAPI, Uvicorn, SQLAlchemy, Pydantic, Azure SDK clients, Redis, and JWT/password hashing libraries.

Useful local credentials:

- Email: `ava@example.org`
- Password: `Password123!`

Optional environment variables:

- `DATABASE_URL`
- `JWT_SECRET`
- `REDIS_URL`
- `AZURE_STORAGE_CONNECTION_STRING`
- `AZURE_COMMUNICATION_CONNECTION_STRING`
- `BACKEND_HOST`
- `BACKEND_PORT`
