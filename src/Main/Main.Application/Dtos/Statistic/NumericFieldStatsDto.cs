using Main.Application.Dtos.Inventories.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Statistic
{
    public record NumericFieldStatsDto
    {
        public int FieldId { get; init; }
        public required string FieldName { get; init; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double AverageValue { get; set; }
        public List<ValueCountDto> ValueDistribution { get; init; } = new();
    }
}
