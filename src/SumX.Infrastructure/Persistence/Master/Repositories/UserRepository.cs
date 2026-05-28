
using Microsoft.AspNetCore.Identity;
using SumX.Application.User.Interface;
using SumX.Domain.Entities;
using SumX.Infrastructure.Persistence.Master.Identity;
using SumX.Infrastructure.Persistence.Master.Mapper;

namespace SumX.Infrastructure.Persistence.Master.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<MasterApplicationUser> _userManager;

        public UserRepository(UserManager<MasterApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            return user is null
                ? null
                : ApplicationUserMapper.ToDomain(user);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user is null
                ? null
                : ApplicationUserMapper.ToDomain(user);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user is not null;
        }

        public async Task<Guid> CreateAsync(ApplicationUser user, string password)
        {
            var identityUser = ApplicationUserMapper.ToIdentity(user);

            var result = await _userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User creation failed: {errors}");
            }

            return identityUser.Id;
        }

        public async Task AssignRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new Exception("User not found");

            user.Role = role;

            await _userManager.UpdateAsync(user);
        }

        public async Task AssignTenantAsync(Guid userId, string tenantId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new Exception("User not found");

            user.TenantId = tenantId;

            await _userManager.UpdateAsync(user);
        }
        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}