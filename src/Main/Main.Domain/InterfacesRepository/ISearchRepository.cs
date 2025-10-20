using Main.Domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.InterfacesRepository
{
    public interface ISearchRepository
    {
        Task<GlobalSearchResult> GlobalSearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<QuickSearchResult> QuickSearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> IsFullTextAvailableAsync(CancellationToken cancellationToken = default);
    }
}
