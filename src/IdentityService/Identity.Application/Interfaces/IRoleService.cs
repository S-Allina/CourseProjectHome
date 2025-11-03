using Identity.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Dto;

namespace Identity.Application.Interfaces
{
    public interface IRoleService
    {
        Task UpdateUsersRoleAsync(IEnumerable<string> userIds, string removeRole, string addRole, CancellationToken cancellationToken);
        Task<ResponseDto> GetAllAsync(IEnumerable<ApplicationUser> users, CancellationToken cancellationToken);
    }
}
