using Main.Domain.entities;

namespace Main.Domain.InterfacesRepository
{
    public interface ISearchRepository
    {
        Task<GlobalSearchResult> GlobalSearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<QuickSearchResult> QuickSearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> IsFullTextAvailableAsync(CancellationToken cancellationToken = default);
        Task<List<UserSearchResult>> SearchUsersAsync(string searchTerm, int limit = 10, CancellationToken cancellationToken = default);
        Task<List<UserSearchResult>> GetUsersDetailsAsync(List<string> userIds, CancellationToken cancellationToken = default);
    }
}
