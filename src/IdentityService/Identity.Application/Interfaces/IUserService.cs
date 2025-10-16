using Identity.Application.Dto;
using Identity.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Dto;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<CurrentUserDto> GetCurrentUserAsync();
        Task<CurrentUserDto> GetByIdAsync(Guid id);
        Task<ResponseDto> GetAllAsync(CancellationToken cancellationToken);
        Task<CurrentUserDto> UpdateAsync(Guid id, UserUpdateRequestDto user);
        Task<ResponseDto> DeleteSomeUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken);
    }
}
