using Main.Application.Dtos.Items.Index;
using Main.Domain.enums.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Items.Create
{
    public record CreateItemFieldValueDto : ItemFieldValueDto
    {
        public bool IsRequired { get; init; }
    }
}
