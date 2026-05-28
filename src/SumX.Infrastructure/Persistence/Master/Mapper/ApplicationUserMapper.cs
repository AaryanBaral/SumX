using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SumX.Domain.Entities;
using SumX.Infrastructure.Persistence.Master.Identity;

namespace SumX.Infrastructure.Persistence.Master.Mapper
{
    public static class ApplicationUserMapper
    {
        public static MasterApplicationUser ToIdentity(ApplicationUser user)
        {
            return new MasterApplicationUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Email, // Identity requirement
                TenantId = user.TenantId,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
        public static ApplicationUser ToDomain(MasterApplicationUser user)
        {
            return user == null
                ? throw new ArgumentNullException(nameof(user))
                : ApplicationUser.CreateTenantUser(
                    id: user.Id,
                    emailAddress: user.Email!, // assuming VO factory
                    tenantId: user.TenantId!,
                    role: user.Role,
                    createdAtUtc: user.CreatedAt);
        }

    }
}