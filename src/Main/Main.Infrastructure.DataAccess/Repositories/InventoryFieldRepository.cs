using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class InventoryFieldRepository : BaseRepository<InventoryField>, IInventoryFieldRepository
    {
        public InventoryFieldRepository(ApplicationDbContext db) : base(db) { }
    }
}
