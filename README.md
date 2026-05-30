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
Update the connection string in the `src/SumX.API/appsettings.json` file to point to your PostgreSQL instance:
```json
{
  "ConnectionStrings": {
    "MasterDb": "Host=127.0.0.1;Port=5432;Database=sumx_master;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Run EF Core Migrations for the Master Database
Apply the initial migration to create the master database structure (Tenants & Identity tables):
```bash
dotnet ef database update --project src/SumX.Infrastructure --startup-project src/SumX.API
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
Once running, you can access the Swagger UI interface at: `http://localhost:5000/swagger` or `https://localhost:5001/swagger`.

---

## 🔄 Tenant Database Creation Flow

When a `SuperAdmin` triggers the `CreateTenantCommand`, the following orchestrations occur automatically:

```
[SuperAdmin JWT] ──> CreateTenant API ──> CreateTenantCommandHandler
                                                      │
                                                      ├──> 1. Store Tenant metadata & DbConnStr in MasterDb
                                                      │
                                                      ├──> 2. Create Dynamic Tenant DB programmatically
                                                      │    (Run tenantDbContext.Database.MigrateAsync())
                                                      │
                                                      └──> 3. Create Tenant Default Admin User in MasterDb
```

1.  **Metadata Persistence**: Tenant properties (ID, name, email, unique 4-character tenant code, and dynamic connection string) are stored in the `Tenants` table of the Master Database.
2.  **Database Provisioning**: The system initializes a dynamic instance of `TenantDbContext` pointing to the newly generated database connection string and programmatically applies EF Core migrations (`Database.MigrateAsync()`) to create the isolated `Employees` table.
3.  **Tenant Admin Creation**: An ASP.NET Core Identity user is created in the Master Database, marked with the corresponding `TenantId` and assigned the `Admin` role.

---

## 📬 Example API Requests

### 1. Authenticate / Login
**POST** `/api/v1.0/auth/login`
```bash
curl -X POST http://localhost:5000/api/v1.0/auth/login \
     -H "Content-Type: application/json" \
     -d '{
       "email": "assessment@yopmail.com",
       "password": "Tester@123"
     }'
```

### 2. Create Tenant (SuperAdmin Only)
**POST** `/api/v1.0/tenants`
```bash
curl -X POST http://localhost:5000/api/v1.0/tenants \
     -H "Authorization: Bearer <SUPERADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Acme Corporation",
       "email": "admin@acme.com",
       "tenantId": "ACME",
       "dbConnStr": "Host=127.0.0.1;Port=5432;Database=sumx_tenant_acme;Username=postgres;Password=postgres"
     }'
```

### 3. Register User (Tenant Admin Only)
**POST** `/api/v1.0/users/register`
```bash
curl -X POST http://localhost:5000/api/v1.0/users/register \
     -H "Authorization: Bearer <TENANT_ADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "email": "employee1@acme.com",
       "password": "Password@123",
       "role": "Employee"
     }'
```

### 4. Create Employee (Tenant Admin Only)
**POST** `/api/v1.0/employees`
```bash
curl -X POST http://localhost:5000/api/v1.0/employees \
     -H "Authorization: Bearer <TENANT_ADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "fullName": "John Doe",
       "emailAddress": "employee1@acme.com"
     }'
```

### 5. Fetch My Information (Employee Only)
**GET** `/api/v1.0/employees/me`
```bash
curl -X GET http://localhost:5000/api/v1.0/employees/me \
     -H "Authorization: Bearer <EMPLOYEE_JWT_TOKEN>"
```
