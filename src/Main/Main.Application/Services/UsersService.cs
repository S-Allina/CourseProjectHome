using AutoMapper;
using Main.Application.Dtos.Common;
using Main.Application.Interfaces;
using Main.Domain.entities.common;
using Main.Domain.Enums.Users;
using Main.Domain.InterfacesRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public bool CheckBlock(string[] ids)
        {
            var id = GetCurrentUserId();
            return ids.Contains(id);
        }

        public async Task<UserDto> GetCurrentUser()
        {
            var id = GetCurrentUserId();
            var user=await _userRepository.GetFirstAsync(u => u.Id == id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> CreateUser(CreateUserRequest request)
        {
            var existingUser = await _userRepository.GetFirstAsync(u=>u.Id==request.Id);
            if (existingUser != null)
                return "User already exists" ;

            var user = _mapper.Map<User>(request);

            await _userRepository.CreateAsync(user);
            return "User created successfully";
        }

        public string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
        }
    }
}
