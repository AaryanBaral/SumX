#!/bin/bash
set -e

until nc -z -v -w30 db 5432; do
  echo "Waiting for PostgreSQL database (db:5432) to be ready..."
  sleep 2
done

echo "Database is ready. Cleaning old migrations to avoid conflicts..."
rm -rf src/SumX.Infrastructure/Persistence/Master/Migrations
rm -rf src/SumX.Infrastructure/Persistence/Tenants/Migrations

echo "Generating fresh Master database migrations..."
dotnet ef migrations add InitialMaster \
  --project src/SumX.Infrastructure/SumX.Infrastructure.csproj \
  --startup-project src/SumX.API/SumX.API.csproj \
  --context MasterDbContext \
  -o Persistence/Master/Migrations

echo "Generating fresh Tenant database migrations..."
dotnet ef migrations add InitialTenant \
  --project src/SumX.Infrastructure/SumX.Infrastructure.csproj \
  --startup-project src/SumX.API/SumX.API.csproj \
  --context TenantDbContext \
  -o Persistence/Tenants/Migrations

echo "Migrations created successfully. Starting SumX API..."
exec dotnet run --project src/SumX.API/SumX.API.csproj --urls http://0.0.0.0:8080
