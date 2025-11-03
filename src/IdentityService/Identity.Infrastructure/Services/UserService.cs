using AutoMapper;
using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Users.Application.Dto;

namespace Identity.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IMainApiClient _mainApiClient;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, IMainApiClient mainApiClient, IRoleService roleService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleService = roleService;
            _mainApiClient = mainApiClient;
        }

        public async Task<ResponseDto> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.Where(u => !u.EmailConfirmed).Select(u => u.Id).ToListAsync(cancellationToken);

            return await DeleteUsersAsync(users, cancellationToken);
        }

        public async Task<ResponseDto> DeleteSomeUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            if (userIds == null) return await GetAllAsync(cancellationToken);
            return await DeleteUsersAsync(userIds, cancellationToken);
        }

        public async Task<ResponseDto> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken);
            return await _roleService.GetAllAsync(users, cancellationToken); ;
        }

        public async Task<ResponseDto> BlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            var result = await UpdateUsersStatusAsync(userIds, user => Statuses.Blocked, cancellationToken);
            await _mainApiClient.NotifyBlockedUsers(userIds.ToArray());
            return result;  
        }

        public async Task<ResponseDto> RoleChange(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            return await UpdateUsersStatusAsync(userIds, user => Statuses.Blocked, cancellationToken);
        }

        public async Task<ResponseDto> UnlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            return await UpdateUsersStatusAsync(userIds, user =>
                user.EmailConfirmed ? Statuses.Activity : Statuses.Unverify, cancellationToken);
        }

        public async Task<ResponseDto> UpdateUsersStatusAsync(IEnumerable<string> userIds, Func<ApplicationUser, Statuses> statusSelector, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userIds?.Any() != true)
                return await GetAllAsync(cancellationToken);

            var usersWithNewStatus = await _userManager.Users.Where(u => userIds.Contains(u.Id)).Select(u => new { UserId = u.Id, NewStatus = statusSelector(u) }).ToListAsync(cancellationToken);

            if (!usersWithNewStatus.Any())
                return await GetAllAsync(cancellationToken);

            var statusGroups = usersWithNewStatus.GroupBy(x => x.NewStatus);

            foreach (var group in statusGroups)
            {
                var userIdsInGroup = group.Select(x => x.UserId).ToList();

                await _userManager.Users.Where(u => userIdsInGroup.Contains(u.Id)).ExecuteUpdateAsync(
                        setter => setter.SetProperty(u => u.Status, group.Key),
                        cancellationToken
                    );
            }
            return await GetAllAsync(cancellationToken);
        }

        public async Task<ResponseDto> UpdateUsersRoleAsync(IEnumerable<string> userIds, Roles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userIds?.Any() != true)
                return await GetAllAsync(cancellationToken);

            var currentRole = role == Roles.User ? Roles.Admin.ToString() : Roles.User.ToString();
            var targetRole = role == Roles.User ? Roles.User.ToString() : Roles.Admin.ToString();

            await _roleService.UpdateUsersRoleAsync(userIds, currentRole, targetRole, cancellationToken);

            return await GetAllAsync(cancellationToken);
        }

        private async Task<ResponseDto> DeleteUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            if (userIds?.Any() != true)
                return await GetAllAsync(cancellationToken);

            var tasks = _userManager.Users.Where(u => userIds.Contains(u.Id)).ExecuteDeleteAsync(cancellationToken);
            await Task.WhenAll(tasks);

            await _mainApiClient.DeleteUserAsync(userIds.ToArray());
            return await GetAllAsync(cancellationToken);
        }

        public async Task<CurrentUserDto> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var user = await GetUserByIdAsync(id);

            return _mapper.Map<CurrentUserDto>(user);
        }

        private async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            return user;
        }
    }
}
