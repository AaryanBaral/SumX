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
        private Guid? _resolvedTenantId;

        public TenantProvider(
            ICurrentUserContext currentUserContext,
            MasterDbContext masterDbContext)
        {
            _currentUserContext = currentUserContext;
            _masterDbContext = masterDbContext;
        }

        public Guid? TenantId => _resolvedTenantId ?? _currentUserContext.TenantId;

        public Task SetTenantAsync(Guid tenantId)
        {
            _resolvedTenantId = tenantId;
            return Task.CompletedTask;
        }

        public async Task<string> GetConnectionStringAsync()
        {
            if (!TenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context is missing for the current request.");
            }

            var tenant = await _masterDbContext.Tenants
                 .AsNoTracking()
                 .FirstOrDefaultAsync(t => t.Id == TenantId.Value);

            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant '{TenantId}' could not be found.");
            }

            return tenant.DatabaseConnectionString;
        }
    }
}
