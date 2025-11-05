using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Items.Index
{
    public record ItemFieldValueFormDto(bool IsRequired, string CreatedById, DateTime CreatedAt, DateTime? UpdatedAt, byte[]? Version) : ItemFieldValueDto
    {
        public string CustomId { get; set; } = string.Empty;
    }
}
