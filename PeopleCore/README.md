# PeopleCore — Enterprise HRMS Starter

A portfolio-grade starter for an Enterprise Human Resource Management System. It includes an ASP.NET Core Web API, SQL Server/EF Core data layer, JWT authentication, role-based authorization, employee and department management, leave approvals, a summary endpoint, and an Angular dashboard.

> This is a working starter/scaffold, not a finished production HR/payroll product. It has not been compiled in this environment because the .NET SDK is not installed here. Run the checks below on a machine with the .NET 8 SDK and Node.js installed.

## Features

- Register/login with password hashing (BCrypt) and JWT tokens
- Employee CRUD with search and pagination; delete is a soft-delete (`IsActive = false`)
- Department listing/creation/deletion with validation
- Leave request creation and HR/Manager review workflow
- Dashboard summary endpoint
- EF Core model configuration, unique indexes, relationships and enum conversion
- Swagger/OpenAPI in Development
- Angular dashboard with login/register, employee listing/search, and employee creation form
- Dockerfile and Docker Compose for API + SQL Server

## Stack

- Backend: C#, ASP.NET Core 8 Web API, Entity Framework Core 8
- Database: Microsoft SQL Server 2022
- Frontend: Angular 18, TypeScript
- Auth: JWT bearer tokens + BCrypt password hashing
- Optional deployment/containerization: Docker, Azure

## Run locally

### 1. Requirements

Install the .NET 8 SDK, Node.js (20+ recommended), npm, and SQL Server (or Docker Desktop).

### 2. Start SQL Server with Docker

From the project root:

```bash
docker compose up -d sqlserver
```

The sample SQL password is for local development only. Change it before using the project anywhere else.

### 3. Configure the API

Open `backend/PeopleCore.Api/appsettings.json` and set a strong, unique JWT key and your SQL Server connection string. Never commit real secrets to GitHub; use .NET User Secrets or environment variables for local secrets.

From `backend/PeopleCore.Api` run:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Open the Swagger page shown in the terminal, normally `https://localhost:<port>/swagger`. The health check is `/health`.

If using the provided Compose setup instead, from the project root run `docker compose up --build`. SQL Server may take a little time to become ready on its first start; if the API exits during initial startup, restart the API container after SQL Server is healthy.

### 4. Run the Angular frontend

Open a second terminal:

```bash
cd frontend
npm install
npm start
```

Open `http://localhost:4200`. The frontend expects the API at `http://localhost:8080`; if your local API runs at a different port, update the `api` value in `frontend/src/app/app.component.ts` and ensure the API CORS origin matches your frontend origin.

## First HR/Admin user

For safety, public registration always creates an `Employee` role. After registering a user, promote that account locally in SQL Server before trying HR-only actions:

```sql
UPDATE dbo.Users SET Role = 'Admin' WHERE Email = 'your-email@example.com';
```

Log out and log back in to receive a token with the new role. Do not expose self-service role changes in a public API. In a real deployment, use a secure admin provisioning process.

## API endpoints

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| GET | `/api/employees?search=&page=1&pageSize=10` | Authenticated |
| GET | `/api/employees/{id}` | Authenticated |
| POST | `/api/employees` | Admin, HR |
| PUT | `/api/employees/{id}` | Admin, HR |
| DELETE | `/api/employees/{id}` | Admin, HR (soft delete) |
| GET | `/api/departments` | Authenticated |
| POST | `/api/departments` | Admin, HR |
| DELETE | `/api/departments/{id}` | Admin, HR |
| GET | `/api/leaverequests?status=Pending` | Authenticated |
| POST | `/api/leaverequests` | Authenticated |
| PATCH | `/api/leaverequests/{id}/review` | Admin, HR, Manager |
| GET | `/api/dashboard/summary` | Admin, HR, Manager |
| GET | `/health` | Public |

Use `Authorization: Bearer <token>` for protected API calls.

## Suggested next improvements

1. Add automated unit and integration tests.
2. Add a proper audit log table and record before/after changes.
3. Add employee-self-service authorization so users can only access their own records where appropriate.
4. Add refresh-token rotation, rate limiting, account lockout and email verification.
5. Add leave overlap checks, business calendars and configurable approval policies.
6. Add Angular routes, guards, reactive forms, accessible validation, and separate feature components.
7. Add CI pipeline, structured telemetry, health checks and deployment configuration.
8. Add a migration strategy and seed-data strategy for production.
9. Add privacy controls, retention policies and security review before storing real employee data.

## Security notes

- Replace all example credentials and the JWT signing key before running outside a local sandbox.
- Never commit secrets or real employee data.
- HTTPS, secure secret storage, rate limiting, CSRF/CORS review, logging redaction and access-control testing are required before production.
- This starter is for learning and portfolio demonstration; it is not legal, payroll, or compliance software.
