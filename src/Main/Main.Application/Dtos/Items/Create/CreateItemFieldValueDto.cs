using Main.Application.Dtos.Items.Index;
using Microsoft.AspNetCore.Http;

namespace Main.Application.Dtos.Items.Create
{
    public record CreateItemFieldValueDto : ItemFieldValueDto
    {
        public bool IsRequired { get; init; }
        public IFormFile? File { get; init; }
    }
}
