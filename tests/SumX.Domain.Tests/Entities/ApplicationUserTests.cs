using System;
using FluentAssertions;
using SumX.Domain.Constants;
using SumX.Domain.Entities;
using SumX.Domain.Exceptions;
using Xunit;

namespace SumX.Domain.Tests.Entities
{
    public class ApplicationUserTests
    {
        [Fact]
        public void CreateSuperAdmin_ShouldSucceed_WhenTenantIdIsNull()
        {
            var createdAt = new DateTime(2026, 05, 27, 12, 0, 0, DateTimeKind.Utc);
            var id = Guid.NewGuid();

            var user = ApplicationUser.CreateSuperAdmin(
                id,
                "superadmin@sumx.com",
                createdAtUtc: createdAt);

            user.Id.Should().Be(id);
            user.Email.Should().Be("superadmin@sumx.com");
            user.TenantId.Should().BeNull();
            user.Role.Should().Be(Roles.SuperAdmin);
            user.CreatedAt.Should().Be(createdAt);
        }

        [Fact]
        public void CreateSuperAdmin_ShouldThrow_WhenTenantIdIsProvided()
        {
            var act = () => ApplicationUser.CreateSuperAdmin(
                Guid.NewGuid(),
                "superadmin@sumx.com",
                tenantId: Guid.NewGuid());

            act.Should().Throw<DomainException>()
                .WithMessage("*must not belong to a tenant*");
        }

        [Fact]
        public void CreateTenantUser_ShouldThrow_WhenAdminHasNoTenantId()
        {
            var act = () => ApplicationUser.CreateTenantUser(
                Guid.NewGuid(),
                "admin@tenant.com",
                Guid.Empty,
                Roles.Admin);

            act.Should().Throw<DomainException>()
                .WithMessage("*must belong to a tenant*");
        }

        [Fact]
        public void CreateTenantUser_ShouldThrow_WhenEmployeeHasNoTenantId()
        {
            var act = () => ApplicationUser.CreateTenantUser(
                Guid.NewGuid(),
                "employee@tenant.com",
                Guid.Empty,
                Roles.Employee);

            act.Should().Throw<DomainException>()
                .WithMessage("*must belong to a tenant*");
        }

        [Fact]
        public void CreateTenantUser_ShouldThrow_WhenRoleIsInvalid()
        {
            var act = () => ApplicationUser.CreateTenantUser(
                Guid.NewGuid(),
                "user@tenant.com",
                Guid.NewGuid(),
                "Manager");

            act.Should().Throw<DomainException>()
                .WithMessage("*Role is invalid*");
        }

        [Fact]
        public void CreateTenantUser_ShouldSucceed_WhenTenantUserIsValid()
        {
            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();

            var user = ApplicationUser.CreateTenantUser(
                id,
                "admin@tenant.com",
                tenantId,
                Roles.Admin);

            user.Id.Should().Be(id);
            user.Email.Should().Be("admin@tenant.com");
            user.TenantId.Should().Be(tenantId);
            user.Role.Should().Be(Roles.Admin);
            user.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void ChangeEmail_ShouldUpdateEmail_WhenNewEmailIsDifferent()
        {
            var user = ApplicationUser.CreateTenantUser(
                Guid.NewGuid(),
                "employee@tenant.com",
                Guid.NewGuid(),
                Roles.Employee);

            user.ChangeEmail("employee.updated@tenant.com");

            user.Email.Should().Be("employee.updated@tenant.com");
        }

        [Fact]
        public void CreateSuperAdmin_ShouldThrow_WhenCreatedAtIsNotUtc()
        {
            var act = () => ApplicationUser.CreateSuperAdmin(
                Guid.NewGuid(),
                "superadmin@sumx.com",
                createdAtUtc: DateTime.Now);

            act.Should().Throw<DomainException>()
                .WithMessage("*must be in UTC*");
        }

        [Fact]
        public void ApplicationUser_ShouldNotExposePublicSetters()
        {
            typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Id))!.GetSetMethod().Should().BeNull();
            typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Email))!.GetSetMethod().Should().BeNull();
            typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.TenantId))!.GetSetMethod().Should().BeNull();
            typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Role))!.GetSetMethod().Should().BeNull();
            typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.CreatedAt))!.GetSetMethod().Should().BeNull();
        }

        [Fact]
        public void ApplicationUser_ShouldInheritAggregateRootIdentityEquality()
        {
            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();

            var first = ApplicationUser.CreateTenantUser(
                id,
                "first@tenant.com",
                tenantId,
                Roles.Admin);

            var second = ApplicationUser.CreateTenantUser(
                id,
                "second@tenant.com",
                tenantId,
                Roles.Admin);

            first.Should().Be(second);
        }
    }
}
