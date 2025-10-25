using Main.Application.Dtos.Inventories.Index;
using Main.Domain.entities.common;

namespace Main.Application.Interfaces
{
    public interface ITagService
    {
        Task<List<TagWithCountDto>> GetAllTagsWithCountAsync();
        Task<List<Tag>> GetPopularTagsAsync(int count = 50);
    }
}
