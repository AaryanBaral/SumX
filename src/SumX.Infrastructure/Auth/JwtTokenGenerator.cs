using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SumX.Application.Auth.Interfaces;
using SumX.Domain.Entities;

namespace SumX.Infrastructure.Auth
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOptions> options)
        {
            _jwtOptions = options.Value;
        }
        public string GenerateToken(ApplicationUser user)
        {
            //adding claims
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new(JwtRegisteredClaimNames.Email, user.Email.ToString()) ,
              new(ClaimTypes.Role, user.Role.ToString()),
              new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            if (user.TenantId is not null)
            {
                claims.Add(
    new Claim(
        "tenant_id",
        user.TenantId.ToString()));
            }
            // initializing signing securoty key from options
            var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            // adding signing creds woth hasing
            var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
            // creating token
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                signingCredentials: signingCredentials,
                expires: expiry);


            // returnning jwt token
            return new JwtSecurityTokenHandler()
                .WriteToken(token);

        }
    }
}