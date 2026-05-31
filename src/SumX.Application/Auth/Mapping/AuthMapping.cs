using SumX.Application.Auth.DTOs;
using SumX.Domain.Entities;

namespace SumX.Application.Auth.Mapping;

internal static class AuthMapping
{
    public static AuthResult ToAuthResult(this ApplicationUser user, string token) =>
        new()
        {
            AccessToken = token,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role,
            TenantId = user.TenantId
        };
}
