Master database migrations live here.

This database contains:
- Tenants
- Users and Identity tables

Example:
```bash
dotnet ef migrations add InitialMaster \
  --project src/SumX.Infrastructure \
  --startup-project src/SumX.API \
  --context MasterDbContext \
  --output-dir Persistence/Master/Migrations
```
