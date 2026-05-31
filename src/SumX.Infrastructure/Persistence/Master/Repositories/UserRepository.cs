using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Exceptions;
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
                throw new AppValidationException(
                    result.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                        error.Code,
                        error.Description)));
            }

            return identityUser.Id;
        }

        public async Task AssignRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new Exception("User not found");

            user.Role = role;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new AppValidationException(
                    updateResult.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                        error.Code,
                        error.Description)));
            }

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    throw new AppValidationException(
                        roleResult.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                            error.Code,
                            error.Description)));
                }
            }
        }

        public async Task AssignTenantAsync(Guid userId, Guid tenantId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new Exception("User not found");

            user.TenantId = tenantId;

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id.ToString());

            if (identityUser is null)
            {
                throw new NotFoundException("User not found");
            }

            identityUser.Email = user.Email;
            identityUser.UserName = user.Email;
            identityUser.TenantId = user.TenantId;
            identityUser.Role = user.Role;
            identityUser.CreatedAt = user.CreatedAt;

            var updateResult = await _userManager.UpdateAsync(identityUser);
            if (!updateResult.Succeeded)
            {
                throw new AppValidationException(
                    updateResult.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                        error.Code,
                        error.Description)));
            }

            var existingRoles = await _userManager.GetRolesAsync(identityUser);
            if (existingRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(identityUser, existingRoles);
                if (!removeResult.Succeeded)
                {
                    throw new AppValidationException(
                        removeResult.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                            error.Code,
                            error.Description)));
                }
            }

            var roleResult = await _userManager.AddToRoleAsync(identityUser, user.Role);
            if (!roleResult.Succeeded)
            {
                throw new AppValidationException(
                    roleResult.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                        error.Code,
                        error.Description)));
            }
        }

        public async Task DeleteAsync(Guid userId)
        {
            var identityUser = await _userManager.FindByIdAsync(userId.ToString());

            if (identityUser is null)
            {
                throw new NotFoundException("User not found");
            }

            var deleteResult = await _userManager.DeleteAsync(identityUser);
            if (!deleteResult.Succeeded)
            {
                throw new AppValidationException(
                    deleteResult.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                        error.Code,
                        error.Description)));
            }
        }

        public async Task DeleteByTenantIdAsync(Guid tenantId)
        {
            var users = await _userManager.Users
                .Where(u => u.TenantId == tenantId)
                .ToListAsync();

            foreach (var user in users)
            {
                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    throw new AppValidationException(
                        deleteResult.Errors.Select(error => new FluentValidation.Results.ValidationFailure(
                            error.Code,
                            error.Description)));
                }
            }
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
