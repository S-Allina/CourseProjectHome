using Main.Domain.entities.Comments;
using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using Main.Domain.entities.item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos
{
   
        public record CreateInventoryDto
        {
            public string Name { get; init; }
            public string Description { get; init; }
            public int? CategoryId { get; init; }
            public string ImageUrl { get; init; }
            public bool IsPublic { get; init; }
            public string CustomIdFormat { get; init; }
        public byte[] Version { get; set; }
        public List<string> Tags { get; init; } = new();
            public List<CreateInventoryFieldDto> Fields { get; init; } = new();
        }
}
