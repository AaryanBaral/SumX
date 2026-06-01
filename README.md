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

---

## 🔄 Tenant Database Creation Flow

```
[SuperAdmin Action] ──> CreateTenant API ──> CreateTenantHandler
                                                       │
                                                       ├──> 1. Provision separate DB (on-the-fly)
                                                       │
                                                       ├──> 2. Create tenant metadata in sumx_master
                                                       │
                                                       └──> 3. Create default Admin user for that tenant
```

1. **Database Provisioning**: PostgreSQL creates a new database (`sumx_tenant_<code_here>`) and applies target migrations (`Employees` table).
2. **Metadata Registration**: Connection details are saved in the `Tenants` metadata table.
3. **Tenant Admin Seeding**: Instantiates an identity user mapped to the tenant context with the `Admin` role.

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
