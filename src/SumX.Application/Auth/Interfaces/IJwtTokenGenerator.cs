using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SumX.Domain.Entities;

namespace SumX.Application.Auth.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user);
    }
}