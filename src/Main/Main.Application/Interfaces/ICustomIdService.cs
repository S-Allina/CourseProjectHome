using Main.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Interfaces
{
    public interface ICustomIdService
    {
        Task<string> GenerateCustomIdAsync(int inventoryId, CancellationToken cancellationToken = default);
        //string GeneratePreview(TemplateConfig template);
        Task<bool> ValidateCustomIdAsync(int inventoryId, string customId, CancellationToken cancellationToken = default);
    }
}
