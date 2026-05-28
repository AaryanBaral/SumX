using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Abstractions;
using SumX.Infrastructure.Persistence.Master;

namespace SumX.Infrastructure.Auth
{
    public class TenantProvider : ITenantProvider
    {
        private readonly ICurrentUserContext _currentUserContext;
        private readonly MasterDbContext _masterDbContext;

        public TenantProvider(
            ICurrentUserContext currentUserContext,
            MasterDbContext masterDbContext)
        {
            _currentUserContext = currentUserContext;
            _masterDbContext = masterDbContext;
        }

        public string? TenantId => _currentUserContext.TenantId;

        public async Task<string> GetConnectionStringAsync()
        {
            if (string.IsNullOrEmpty(TenantId))
            {
                throw new InvalidOperationException("Tenant context is missing for the current request.");
            }

            var tenant = await _masterDbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == TenantId || t.TenantId == TenantId);

            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant '{TenantId}' could not be found.");
            }

            return tenant.DatabaseConnectionString;
        }
    }
}
