using SumX.Application.Users.DTOs;
using SumX.Domain.Entities;

namespace SumX.Application.Users.Mapping
{
    internal static class UserMapping
    {
        public static UserDto ToDto(this ApplicationUser user) =>
            new(user.Id, user.Email, user.Role, user.TenantId);
    }
}
