using Main.Domain.entities.common;
using Main.Domain.InterfacesRepository;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        public TagRepository(ApplicationDbContext db) : base(db) { }
    }
}
