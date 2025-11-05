using Main.Application.Dtos.Inventories.Index;

namespace Main.Application.Interfaces
{
    public interface IInventoryStatsService
    {
        Task<InventoryStatsDto> GetInventoryStatsAsync(int inventoryId);
        Task<List<NumericFieldStatsDto>?> GetNumericFieldStatsAsync(int inventoryId);
        Task<List<TextFieldStatsDto>?> GetTextFieldStatsAsync(int inventoryId);
    }
}
