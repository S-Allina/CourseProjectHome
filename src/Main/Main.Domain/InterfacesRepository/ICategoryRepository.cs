using Main.Domain.entities.Comments;
using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.InterfacesRepository
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
    }
}
