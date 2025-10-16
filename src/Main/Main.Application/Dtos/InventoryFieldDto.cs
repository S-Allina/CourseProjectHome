using Main.Domain.enums.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos
{
    public record InventoryFieldDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public FieldType FieldType { get; init; }
        public int OrderIndex { get; init; }
        public bool IsVisibleInTable { get; init; }
        public bool IsRequired { get; init; }
    }
}
