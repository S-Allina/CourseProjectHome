using Main.Application.Dtos;
using Main.Domain.entities.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Interfaces
{
    public interface ITagService
    {
        Task<List<TagWithCountDto>> GetAllTagsWithCountAsync();
        Task<List<Tag>> GetPopularTagsAsync(int count = 50);
    }
}
