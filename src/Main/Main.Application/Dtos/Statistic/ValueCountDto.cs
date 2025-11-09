using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Statistic
{
    public record ValueCountDto
    {
        public required string Value { get; init; }
        public int Count { get; init; }
    }
}
