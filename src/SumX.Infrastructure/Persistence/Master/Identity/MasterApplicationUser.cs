using System;
using Microsoft.AspNetCore.Identity;

namespace SumX.Infrastructure.Persistence.Master.Identity;

public sealed class MasterApplicationUser : IdentityUser<Guid>
{
    public Guid? TenantId { get; set; }

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
