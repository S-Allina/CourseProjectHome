using Main.Domain.enums.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Statistic
{
    public record FieldStatsDto
    {
        public int FieldId { get; init; }
        public required string FieldName { get; init; }
        public FieldType FieldType { get; init; }

        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? AverageValue { get; set; }

        public Dictionary<string, int>? ValueCounts { get; set; }
        public int? UniqueValuesCount { get; set; }

        public int? EmptyValuesCount { get; init; }
        public int? NonEmptyValuesCount { get; init; }
    }
}
