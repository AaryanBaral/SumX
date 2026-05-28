using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SumX.Application.Common.Abstractions;

namespace SumX.Infrastructure.Auth
{
    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId
        {
            get
            {
                var sub = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User?.FindFirst("sub")?.Value;
                
                return Guid.TryParse(sub, out var guid) ? guid : Guid.Empty;
            }
        }

        public string? TenantId => User?.FindFirst("tenant_id")?.Value;

        public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
