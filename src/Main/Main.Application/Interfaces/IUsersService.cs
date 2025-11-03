using Main.Application.Dtos.Common;
using Main.Domain.entities.common;
using Main.Domain.Enums.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Interfaces
{
    public interface IUsersService
    {
        string GetCurrentUserId();
        string GetCurrentUserRole();
        Task<UserDto> GetCurrentUser();
        bool CheckBlock(string[] ids);
        Task<string> CreateUser(CreateUserRequest request);
        Task<bool> DeleteUsersAsync(string[] ids);
    }
}
