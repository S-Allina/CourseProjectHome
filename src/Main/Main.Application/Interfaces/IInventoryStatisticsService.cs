using Main.Application.Dtos.Inventories.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Interfaces
{
    public interface IInventoryStatsService
    {
        Task<InventoryStatsDto> GetInventoryStatsAsync(int inventoryId);
        Task<List<NumericFieldStatsDto>?> GetNumericFieldStatsAsync(int inventoryId);
        Task<List<TextFieldStatsDto>?> GetTextFieldStatsAsync(int inventoryId);
    }
}
