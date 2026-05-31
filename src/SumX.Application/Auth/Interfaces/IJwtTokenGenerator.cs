using SumX.Domain.Entities;

namespace SumX.Application.Auth.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}
