using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SumX.Application.Auth.DTOs
{
    public class AuthResult
    {
        public string AccessToken { get; init; } = string.Empty;

        public Guid UserId { get; init; }

        public string Email { get; init; } = string.Empty;

        public string Role { get; init; } = string.Empty;

        public Guid? TenantId { get; init; }
    }
}