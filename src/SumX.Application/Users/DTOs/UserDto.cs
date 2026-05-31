using System;

namespace SumX.Application.Users.DTOs
{
    public sealed record UserDto(
        Guid Id,
        string Email,
        string Role,
        Guid? TenantId);
}
