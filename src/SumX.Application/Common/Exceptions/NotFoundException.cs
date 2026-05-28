using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SumX.Application.Common.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}