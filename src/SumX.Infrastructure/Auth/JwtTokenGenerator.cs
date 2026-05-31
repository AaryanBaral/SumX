using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SumX.Application.Auth;
using SumX.Application.Auth.Interfaces;
using SumX.Domain.Entities;
using SumX.Infrastructure.Persistence.Master;

namespace SumX.Infrastructure.Auth
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly MasterDbContext _masterDbContext;

        public JwtTokenGenerator(IOptions<JwtSettings> options, MasterDbContext masterDbContext)
        {
            _jwtSettings = options.Value;
            _masterDbContext = masterDbContext;
        }

        public string GenerateToken(ApplicationUser user)
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

                // Fetch 4-character tenant code from the database
                var tenant = _masterDbContext.Tenants.FirstOrDefault(t => t.Id == user.TenantId.Value);
                if (tenant != null)
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
}