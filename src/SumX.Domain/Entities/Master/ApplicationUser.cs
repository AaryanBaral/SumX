using SumX.Domain.Constants;
using SumX.Domain.Exceptions;
using SumX.SharedKernel.Entities;

namespace SumX.Domain.Entities;

public sealed class ApplicationUser : AggregateRoot
{
    private ApplicationUser(
        Guid id,
        string emailAddress,
        string? tenantId,
        string role,
        DateTime createdAt)
        : base(ValidateId(id))
    {
        Email = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        TenantId = NormalizeTenantId(tenantId);
        Role = ValidateRole(role);
        CreatedAt = EnsureUtc(createdAt);

        EnforceTenantIsolation(Role, TenantId);
    }

    public string? TenantId { get; }

    public string Role { get; }

    public DateTime CreatedAt { get; }

    public string Email { get; private set; }

    public static ApplicationUser CreateSuperAdmin(
        Guid id,
        string emailAddress,
        string? tenantId = null,
        DateTime? createdAtUtc = null) =>
        new(
            id,
            emailAddress,
            tenantId,
            Roles.SuperAdmin,
            createdAtUtc ?? DateTime.UtcNow);

    public static ApplicationUser CreateTenantUser(
        Guid id,
        string emailAddress,
        string tenantId,
        string role,
        DateTime? createdAtUtc = null) =>
        new(
            id,
            emailAddress,
            tenantId,
            role,
            createdAtUtc ?? DateTime.UtcNow);

    private static string ValidateRole(string role)
    {
        if (!Roles.IsValid(role))
        {
            throw new DomainException("Role is invalid.");
        }

        return role;
    }

    public void ChangeEmail(string newEmail)
    {
        Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
    }

    private static Guid ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("User id cannot be empty.");
        }

        return id;
    }

    private static string? NormalizeTenantId(string? tenantId) =>
        string.IsNullOrWhiteSpace(tenantId) ? null : tenantId.Trim();

    private static DateTime EnsureUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc
            ? value
            : throw new DomainException("CreatedAt must be in UTC.");

    private static void EnforceTenantIsolation(string role, string? tenantId)
    {
        if (string.Equals(role, Roles.SuperAdmin, StringComparison.Ordinal) && tenantId is not null)
        {
            throw new DomainException("SuperAdmin must not belong to a tenant.");
        }

        if (!string.Equals(role, Roles.SuperAdmin, StringComparison.Ordinal) && tenantId is null)
        {
            throw new DomainException("Admin and Employee users must belong to a tenant.");
        }
    }
}
