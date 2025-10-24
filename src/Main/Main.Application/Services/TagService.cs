using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Domain.entities.common;
using Main.Domain.InterfacesRepository;

namespace Main.Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagWithCountDto>> GetAllTagsWithCountAsync()
        {
            var tags = await _tagRepository.GetAllAsync();

            return tags.Select(t => new TagWithCountDto
            {
                TagId = t.Id,
                Name = t.Name,
                InventoryCount = t.InventoryTags.Count(),
                TotalUsageCount = t.InventoryTags.Count()
            })
            .Where(t => t.InventoryCount > 0)
            .OrderByDescending(t => t.InventoryCount)
            .ToList();
        }

        public async Task<List<Tag>> GetPopularTagsAsync(int count = 50)
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.OrderByDescending(t => t.InventoryTags.Count())
            .Take(count)
            .ToList();
        }
    }
}
