# SumX - Multi-Tenant Web API

A secure and maintainable Multi-Tenant Web API built with **ASP.NET Core 8** and **PostgreSQL**. Features a multi-database tenant architecture (isolated database per tenant) with dynamic runtime connection string resolution and role-based access control.

---

## 🏛️ Architecture Overview

The project is structured according to **Clean Architecture** and **CQRS**:

*   **`SumX.Domain` (Core)**: Pure business models (`Tenant`, `Employee`, `ApplicationUser`) and domain invariants.
*   **`SumX.Application`**: Implements MediatR CQRS handlers, DTO definitions, FluentValidation rules, and abstractions.
*   **`SumX.Infrastructure`**: EF Core databases (`MasterDbContext`, `TenantDbContext`), Microsoft Identity, and JWT generation services.
*   **`SumX.API`**: REST API endpoints, versioning configs, Swagger, and dynamic tenant middleware.

---

## ⚙️ Setup Instructions

### Single-Command Setup (Docker Compose - Recommended)

To spin up the PostgreSQL database, dynamically generate/apply migrations, seed default data, and start the API, run:

```bash
docker compose up --build
```

- **API Endpoint**: `http://localhost:8080`
- **Swagger Documentation**: `http://localhost:8080/swagger`
- **Master Database Connection**: `Host=localhost;Port=5432;Database=sumx_master;Username=postgres;Password=postgresPassword`

---

## 🔑 Initial SuperAdmin Credentials

Upon database creation, a default system-wide administrator is automatically seeded:

*   **Email (Username)**: `assessment@yopmail.com`
*   **Password**: `Tester@123`
*   **Role**: `SuperAdmin`

## 🔄 Dynamic Database Architecture & Tenant Provisioning Flow

This system uses a **multi-database tenant isolation architecture**, routing queries to separate tenant-specific PostgreSQL databases dynamically at runtime, and provisioning databases on-the-fly when new tenants are registered.

---

### 1. Dynamic Database Routing (Runtime Request Flow)

When an API request targeting a tenant-specific resource (e.g. `/employees`) arrives, the database connection is resolved dynamically using the following lifecycle:

```
[HTTP Request with JWT]
          │
          ▼
1. [TenantResolutionMiddleware] ──> Extracts "tenant_id" claim from validated JWT
          │
          ▼
2. [ITenantProvider] (Scoped)  ──> Stores and locks the resolved Tenant ID for the request
          │
          ▼
3. [MasterDbContext]           ──> TenantProvider queries Master Db Tenants table for connection string (DbConnStr)
          │
          ▼
4. [TenantDbContext]           ──> OnConfiguring() resolves DbConnStr from TenantProvider & opens connection dynamically
```

#### Key Architecture Roles:
*   **`TenantResolutionMiddleware`**: After authentication this midlwware reads the custom `"tenant_id"` claim from the decrypted JWT payload, and registers it into the scoped `ITenantProvider`.
*   **`ITenantProvider` (Scoped)**: Acts as the single source of truth for the active request's tenant context. Its `GetConnectionStringAsync()` method queries `MasterDbContext` `Tenants` table to fetch the matching tenant connection string.
*   **`TenantDbContext` (Scoped)**: Dynamically overrides EF Core's `OnConfiguring` pipeline. Instead of a hardcoded string, it resolves `ITenantProvider.GetConnectionStringAsync()` at runtime, ensuring complete SQL isolation.

---

### 2. Dynamic Database Provisioning (On-The-Fly Creation Flow)

When a `SuperAdmin` registers a new tenant via `POST /api/v1.0/tenants`, the `CreateTenantCommandHandler` orchestrates provisioning:

```
[SuperAdmin JWT] ──> CreateTenant API ──> CreateTenantHandler
                                                       │
                                                       ├──> 1. Build Connection String (ITenantConnectionStringBuilder)
                                                       │
                                                       ├──> 2. Provision Database on PostgreSQL (ITenantDatabaseService)
                                                       │
                                                       ├──> 3. Register Tenant Metadata in Master DB (Tenants table)
                                                       │
                                                       └──> 4. Seed default Tenant Admin in Master DB (Users table)
```

#### Step-by-Step Provisioning Details:
1.  **Build Connection String**: `ITenantConnectionStringBuilder` takes the unique 4-character tenant code (e.g., `ACME`), normalizes it, and constructs a connection string pointing to a new database name: `sumx_tenant_acme`.
2.  **Database Provisioning**: `ITenantDatabaseService` connects to PostgreSQL, executes a `CREATE DATABASE` statement, and triggers `Database.MigrateAsync()` on a temporary `TenantDbContext` instance to dynamically build the `Employees` table in the new database.
3.  **Metadata Registration**: Inserts a new record in the Master database's `Tenants` table containing the `Name`, `EmailAddress`, the unique `TenantId` (code), and the resolved `DbConnStr`.
4.  **Admin Seeding**: Creates a standard ASP.NET Identity user in the Master database `Users` table, assigns the `Admin` role, and binds their `TenantId` foreign key to the newly created tenant's primary key to lock their access.

*Note: The provisioning handler has built-in transaction rollback. If creating the database, registering metadata, or seeding the default Admin fails, it automatically cleans up all partially created assets (drops the database, deletes the admin user, and removes the tenant row).*

---

## 📬 Example API Requests

### 1. Login (Authenticate)
**POST** `/api/v1.0/auth/login`
```bash
curl -X POST http://localhost:8080/api/v1.0/auth/login \
     -H "Content-Type: application/json" \
     -d '{
       "email": "assessment@yopmail.com",
       "password": "Tester@123"
     }'
```

### 2. Create Tenant (SuperAdmin Only)
**POST** `/api/v1.0/tenants`
```bash
curl -X POST http://localhost:8080/api/v1.0/tenants \
     -H "Authorization: Bearer <SUPERADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Acme Corp",
       "email": "admin@acme.com",
       "tenantId": "ACME",
       "adminPassword": "AdminPassword@123"
     }'
```

### 3. Register Tenant User (Tenant Admin Only)
**POST** `/api/v1.0/users/register`
```bash
curl -X POST http://localhost:8080/api/v1.0/users/register \
     -H "Authorization: Bearer <TENANT_ADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "email": "employee@acme.com",
       "password": "Password@123",
       "role": "Employee"
     }'
```

### 4. Create Employee (Tenant Admin Only)
**POST** `/api/v1.0/employees`
```bash
curl -X POST http://localhost:8080/api/v1.0/employees \
     -H "Authorization: Bearer <TENANT_ADMIN_JWT_TOKEN>" \
     -H "Content-Type: application/json" \
     -d '{
       "fullName": "Jane Doe",
       "email": "employee@acme.com"
     }'
```

### 5. Fetch Profile (Employee Only)
**GET** `/api/v1.0/employees/me`
```bash
curl -X GET http://localhost:8080/api/v1.0/employees/me \
     -H "Authorization: Bearer <EMPLOYEE_JWT_TOKEN>"
```
