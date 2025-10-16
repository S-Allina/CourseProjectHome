using AutoMapper;
using FluentEmail.Core;
using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Users.Application.Dto;
using Identity.Domain.Enums;

namespace Identity.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<UserService> logger)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto> DeleteSomeUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            if (userIds == null) return await GetAllAsync(cancellationToken);

            return await DeleteUsersAsync(userIds, cancellationToken);
        }

        public async Task<ResponseDto> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken);

            return new ResponseDto { Result = _mapper.Map<IEnumerable<CurrentUserDto>>(users) };
        }

        public async Task<ResponseDto> BlockUser(IEnumerable<string> userIds, CancellationToken cancellationToken)
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

            var usersToUpdate = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync(cancellationToken);

            foreach (var user in usersToUpdate)
            {
                user.Status = statusSelector(user);

                await _userManager.UpdateAsync(user);
            }

            return await GetAllAsync(cancellationToken);
        }

        private async Task<ResponseDto> DeleteUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            if (userIds?.Any() != true)
                return await GetAllAsync(cancellationToken);

            var tasks = _userManager.Users.Where(u => userIds.Contains(u.Id)).ExecuteDeleteAsync(cancellationToken);
            await Task.WhenAll(tasks);

            return await GetAllAsync(cancellationToken);
        }

        public async Task<CurrentUserDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user by id");

            var user = await GetUserByIdAsync(id.ToString());

            _logger.LogInformation("User found");

            return _mapper.Map<CurrentUserDto>(user);
        }

        //public async Task<IEnumerable<CurrentUserDto>> GetAllAsync()
        //{
        //    _logger.LogInformation("Getting users");

        //    var users = await _userManager.Users.ToListAsync();

        //    _logger.LogInformation("User found");

        //    var result = _mapper.Map<IEnumerable<CurrentUserDto>>(users);

        //    return result;
        //}

        public async Task<CurrentUserDto> GetCurrentUserAsync()
        {
            var userId = _currentUserService.GetUserId();

            var user = await GetUserByIdAsync(userId);

            _logger.LogInformation("User found");

            return _mapper.Map<CurrentUserDto>(user);
        }

        public async Task<CurrentUserDto> UpdateAsync(Guid id, UserUpdateRequestDto requestDto)
        {
            _logger.LogInformation("Updatting user with userId: {userId}", id.ToString());

            var user = await GetUserByIdAsync(id.ToString());

            user.FirstName= requestDto.FirstName;
            user.LastName= requestDto.LastName;
            user.PhoneNumber = requestDto.PhoneNumber;

            await _userManager.UpdateAsync(user);
            
            return _mapper.Map<CurrentUserDto>(user);
        }
        
        private async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }

            return user;
        }
    }
}
