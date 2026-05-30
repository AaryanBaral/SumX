using System;

namespace SumX.Application.Common.Abstractions;

public interface ICurrentUserContext
{
    Guid UserId { get; }

    Guid? TenantId { get; }

    string Role { get; }
    string Email {get;}
}
