using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IMainApiClient
    {
        Task<bool> CreateUserAsync(string userId, string firstName, string lastName, string email);
        Task<bool> NotifyBlockedUsers(string[] blockedUserIds);
        Task<bool> DeleteUserAsync(string[] ids);
    }
}
