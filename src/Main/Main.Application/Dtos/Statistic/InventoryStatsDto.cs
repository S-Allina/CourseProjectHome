using Main.Application.Dtos.Inventories.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Statistic
{
    public record InventoryStatsDto
    {
        public int TotalItems { get; init; }
        public int TotalFields { get; init; }
        public DateTime? OldestItemDate { get; init; }
        public DateTime? NewestItemDate { get; init; }
        public List<FieldStatsDto>? FieldStatistics { get; init; }
    }
}
