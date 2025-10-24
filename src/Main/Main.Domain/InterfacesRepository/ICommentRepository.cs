using Main.Domain.entities.Comments;

namespace Main.Domain.InterfacesRepository
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByInventoryAsync(int inventoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count, CancellationToken cancellationToken = default);
    }
}
