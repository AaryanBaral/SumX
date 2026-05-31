using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SumX.Application.Auth;
using SumX.Application.Common.Abstractions.Security;
using SumX.Application.Common.Abstractions.Persistence.Master;
using SumX.Domain.Entities;

namespace SumX.Infrastructure.Auth;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly ITenantRepository _tenantRepository;

    public JwtTokenGenerator(IOptions<JwtSettings> options, ITenantRepository tenantRepository)
    {
        _jwtSettings = options.Value;
        _tenantRepository = tenantRepository;
    }

    public async Task<string> GenerateTokenAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (user.TenantId is not null)
        {
            claims.Add(new Claim("tenant_id", user.TenantId.Value.ToString()));

            var tenant = await _tenantRepository.GetByIdAsync(user.TenantId.Value);
            if (tenant is not null)
            {
                claims.Add(new Claim("tenant_code", tenant.TenantId));
            }
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            signingCredentials: signingCredentials,
            expires: expiry);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
