using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db) { }
    }
}
