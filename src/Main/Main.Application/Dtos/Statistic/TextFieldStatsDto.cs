using Main.Application.Dtos.Inventories.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Statistic
{
    public record TextFieldStatsDto
    {
        public int FieldId { get; init; }
        public required string FieldName { get; init; }
        public List<ValueCountDto> TopValues { get; init; } = new();
        public int UniqueValuesCount { get; set; }
    }
}
