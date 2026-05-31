# SumX - Multi-Tenant Web API

A secure, performant, and maintainable Multi-Tenant Web API built using **ASP.NET Core 8**, **Entity Framework Core**, and **PostgreSQL**. The solution features a robust multi-database tenant architecture, providing separate isolated databases per tenant with runtime connection string resolution, dynamic database provisioning, and JWT role-based access control.

---

## 🏛️ Architecture Overview

The system is designed following the principles of **Clean Architecture** and **CQRS (Command Query Responsibility Segregation)**:

```
┌────────────────────────────────────────────────────────┐
│                        SumX.API                        │ (HTTP, Controllers, Middlewares)
└───────────────────────────┬────────────────────────────┘
                            ▼
┌────────────────────────────────────────────────────────┐
│                    SumX.Application                    │ (CQRS, MediatR, DTOs, Interfaces, Validators)
└───────────────────────────┬────────────────────────────┘
                            ▼
┌────────────────────────────────────────────────────────┐
│                   SumX.Infrastructure                  │ (EF Core Contexts, Repositories, Identity, JWT)
└───────────────────────────┬────────────────────────────┘
                            ▼
┌────────────────────────────────────────────────────────┐
│                       SumX.Domain                      │ (Entities, Value Objects, Domain Exceptions)
└────────────────────────────────────────────────────────┘
```

### Key Components

*   **`SumX.Domain`**: Contains pure business entities (`Tenant`, `Employee`, `ApplicationUser`) and domain rules. Free of external dependencies.
*   **`SumX.Application`**: Implements MediatR CQRS pattern, defining request pipelines, validation behaviors, repository interfaces, and use-case handlers.
*   **`SumX.Infrastructure`**:
    *   **MasterDbContext**: Interacts with the main database storing metadata for all tenants and global identity users.
    *   **TenantDbContext**: Connects dynamically at runtime to individual tenant databases containing tenant-specific tables (like `Employees`).
    *   **Dynamic Connection Resolution**: Utilizes `ITenantProvider` and `ICurrentUserContext` to inspect the incoming JWT claim (`tenant_id`), retrieve the corresponding connection string, and apply it to `TenantDbContext` per request.
    *   **EF Core Performance Optimizations**: All query methods accept a `trackChanges` parameter (defaulting to `false`) to automatically utilize `.AsNoTracking()` for high-throughput, read-only optimized requests.
*   **`SumX.API`**: Contains REST endpoints, Swagger documentation, and dynamic HTTP request middleware.

---

## 🔑 Initial SuperAdmin Credentials

A default global system administrator can be seeded to bootstrap tenant creation:

*   **Username (Email)**: `assessment@yopmail.com`
*   **Password**: `Tester@123`
*   **TenantId**: `null`
*   **Role**: `SuperAdmin`

---

## ⚙️ Setup Instructions

### 1. Prerequisites
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [PostgreSQL Database Server](https://www.postgresql.org/download/)

### 2. Configure Database Connections
Update the connection string in `src/SumX.API/appsettings.json` to point to your PostgreSQL instance:
```json
{
  "ConnectionStrings": {
    "MasterDb": "Host=127.0.0.1;Port=5432;Database=master_db;Username=postgres;Password=postgres"
  }
}
```

### 3. Run EF Core Migrations for the Master Database (optional)
Migrations are applied automatically on startup via `Database.MigrateAsync()`. To apply them manually instead:
```bash
dotnet ef database update --project src/SumX.Infrastructure --startup-project src/SumX.API --context MasterDbContext
```

### 4. Seed the SuperAdmin User
Execute the application with the `seed` command-line argument to seed roles and the initial SuperAdmin:
```bash
dotnet run --project src/SumX.API seed
```

### 5. Start the API Server
Launch the development server:
```bash
dotnet run --project src/SumX.API
```
Once running, access Swagger at:
*   **HTTP**: `http://localhost:5290/swagger`
*   **HTTPS**: `https://localhost:7101/swagger`

(Ports come from `src/SumX.API/Properties/launchSettings.json`.)

---

## 🔄 Tenant Database Creation Flow

When a `SuperAdmin` triggers `CreateTenantCommand`, the following orchestration runs automatically:

```
[SuperAdmin JWT] ──> CreateTenant API ──> CreateTenantCommandHandler
                                                      │
                                                      ├──> 1. Provision tenant DB (MigrateAsync)
                                                      │
                                                      ├──> 2. Store tenant metadata in MasterDb
                                                      │
                                                      └──> 3. Create tenant Admin user in MasterDb
```

1.  **Database Provisioning**: Connection string is derived from `MasterDb` credentials and tenant code (`sumx_tenant_{code}`). PostgreSQL creates the database if needed, then EF Core migrations run (`MigrateAsync()`).
2.  **Metadata Persistence**: Tenant metadata (including the internal connection string) is stored in the Master database `Tenants` table. API responses expose only `databaseName`, never credentials or full connection strings.
3.  **Tenant Admin Creation**: An Identity user is created in the Master database with the tenant `TenantId` and `Admin` role.

If step 2 or 3 fails after provisioning, the handler compensates by deleting any created admin user, removing the tenant row, and dropping the tenant database.

**Delete tenant**: Removes all master-db users for that tenant, drops the tenant PostgreSQL database, then deletes the tenant row.

---

## 👤 Employee Users: Registration and Tenant Data

Employees exist in two places:

| Store | Purpose |
|-------|---------|
| **Master DB** (`ApplicationUser`) | Login, JWT, `tenant_id` claim |
| **Tenant DB** (`Employees`) | Profile used by `/employees/me` and admin employee APIs |

### Automatic provisioning (default)

When a tenant **Admin** registers a user with role `Employee` via `POST /api/v1.0/users/register` (or `POST /api/v1.0/auth/register`), the API also creates a matching row in the tenant `Employees` table (display name defaults to the email local-part; email matches the registered address). No separate create-employee call is required for basic login and `/employees/me`.

### Manual two-step flow (optional)

You can still manage employees explicitly:

1. **Register** the Identity user (`role`: `Employee`).
2. **Create employee** with `POST /api/v1.0/employees` if you need a custom `fullName` or registered the user before auto-provisioning existed.

Admins registering with role `Admin` only create a Master DB user (no `Employees` row).

---

## 📬 Example API Requests

Base URL (development): `http://localhost:5290` or `https://localhost:7101`

JSON property names match the API request models (camelCase in JSON).

### 1. Authenticate / Login
**POST** `/api/v1.0/auth/login`

Invalid credentials return **401 Unauthorized**.

```bash
curl -X POST http://localhost:5290/api/v1.0/auth/login \
     -H "Content-Type: application/json" \
     -d '{
       "email": "assessment@yopmail.com",
       "password": "Tester@123"
     }'
```

### 2. Create Tenant (SuperAdmin Only)
**POST** `/api/v1.0/tenants`

Required fields: `name`, `email`, `tenantId` (exactly 4 characters), `adminPassword` (min 6 characters). The tenant database name is auto-derived (`sumx_tenant_acme` for code `ACME`).

Set `Jwt:SecretKey` or environment variable `SUMX_JWT_SECRET` (minimum 32 characters). Development defaults are in `appsettings.Development.json`. Use Swagger **Authorize** with `Bearer <token>` after login.

```bash
curl -X POST http://localhost:5290/api/v1.0/tenants \
     -H "Authorization: Bearer <SUPERADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Acme Corporation",
       "email": "admin@acme.com",
       "tenantId": "ACME",
       "adminPassword": "Admin@123"
     }'
```

### 3. Register User (Tenant Admin Only)
**POST** `/api/v1.0/users/register`

Password must meet complexity rules (uppercase, lowercase, digit, non-alphanumeric). `role` must be `Admin` or `Employee`.

```bash
curl -X POST http://localhost:5290/api/v1.0/users/register \
     -H "Authorization: Bearer <TENANT_ADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "email": "employee1@acme.com",
       "password": "Password@123",
       "role": "Employee"
     }'
```

The same endpoint is available at **POST** `/api/v1.0/auth/register` (also Admin-only).

### 4. Create Employee (Tenant Admin Only, optional)
**POST** `/api/v1.0/employees`

Use when you need an explicit `fullName` or did not register via the Employee auto-provisioning path.

```bash
curl -X POST http://localhost:5290/api/v1.0/employees \
     -H "Authorization: Bearer <TENANT_ADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "fullName": "John Doe",
       "email": "employee1@acme.com"
     }'
```

### 5. Fetch My Information (Employee Only)
**GET** `/api/v1.0/employees/me`

```bash
curl -X GET http://localhost:5290/api/v1.0/employees/me \
     -H "Authorization: Bearer <EMPLOYEE_JWT_TOKEN>"
```
