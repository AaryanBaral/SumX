using System;

namespace SumX.Application.Common.Abstractions.Security;

public interface ICurrentUserContext
{
    Guid UserId { get; }

    Guid? TenantId { get; }

    string Role { get; }
    string Email {get;}
}
