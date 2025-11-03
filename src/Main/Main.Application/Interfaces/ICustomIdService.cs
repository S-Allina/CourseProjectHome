namespace Main.Application.Interfaces
{
    public interface ICustomIdService
    {
        Task<string> GenerateCustomIdAsync(int inventoryId, CancellationToken cancellationToken = default);
        Task<bool> ValidateCustomIdAsync(int inventoryId, string customId, CancellationToken cancellationToken = default);
    }
}
