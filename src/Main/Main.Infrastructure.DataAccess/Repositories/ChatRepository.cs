using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class ChatRepository : BaseRepository<ChatMessage>, IChatRepository
    {
        public ChatRepository(ApplicationDbContext db) : base(db) { }
    }
}
