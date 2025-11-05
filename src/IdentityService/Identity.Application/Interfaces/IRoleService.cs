using Identity.Domain.Entity;
using Users.Application.Dto;

namespace Identity.Application.Interfaces
{
    public interface IRoleService
    {
        Task UpdateUsersRoleAsync(IEnumerable<string> userIds, string removeRole, string addRole, CancellationToken cancellationToken);
        Task<ResponseDto> GetAllAsync(IEnumerable<ApplicationUser> users, CancellationToken cancellationToken);
    }
}
