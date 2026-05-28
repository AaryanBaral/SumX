namespace SumX.Application.Common.Abstractions;

public interface ICurrentUserContext
{
    Guid UserId { get; }

    string? TenantId { get; }

    string Role { get; }
}
