using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using Main.Domain.enums.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos
{
    public record ItemFieldValueDto
    {
        public int InventoryFieldId { get; init; }
        public string FieldName { get; init; }
        public FieldType FieldType { get; init; }
        public string? TextValue { get; init; }
        public string? MultilineTextValue { get; init; }
        public decimal? NumberValue { get; init; }
        public string? FileUrl { get; init; }
        public bool? BooleanValue { get; init; }
    }
}
