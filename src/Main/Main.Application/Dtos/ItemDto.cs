using Main.Domain.entities.Comments;
using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos
{
    public record ItemDto
    {
        public int Id { get; init; }
        public int InventoryId { get; init; }
        public string CustomId { get; init; }
        public string CreatedById { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<ItemFieldValueDto> FieldValues { get; init; } 
    }
}
