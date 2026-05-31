using SumX.Application.Common.Abstractions.Security;
using SumX.Domain.Entities;

namespace SumX.Application.Common.Abstractions.Security;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}