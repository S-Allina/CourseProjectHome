using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entity
{
    public record JwtSettings
    {
        public string? Key { get; init; }
        public string ValidIssuer { get; init; }
        public string ValidAudience { get; init; }
        public double Expires { get; init; }
    }
}
