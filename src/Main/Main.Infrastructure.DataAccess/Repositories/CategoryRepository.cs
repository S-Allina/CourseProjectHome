using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db) : base(db) { }
    }
}
