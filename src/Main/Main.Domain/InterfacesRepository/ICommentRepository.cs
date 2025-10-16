using Main.Domain.entities.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.InterfacesRepository
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByInventoryAsync(int inventoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count, CancellationToken cancellationToken = default);
    }
}
