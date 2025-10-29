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
        public string GetCurrentUserId();
        public string GetCurrentUserRole();
        public Theme GetThemeCurentUser();
        Task<UserDto> GetCurrentUser();
    }
}
