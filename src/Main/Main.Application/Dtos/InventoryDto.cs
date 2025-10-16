using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos
{
    public record InventoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string CategoryId { get; init; }
        public string OwnerId { get; init; }
        public string ImageUrl { get; init; }
        public bool IsPublic { get; init; }
        public string CustomIdFormat { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<string> Tags { get; init; } = new();
        public List<InventoryFieldDto> Fields { get; init; } = new();
    }
}
