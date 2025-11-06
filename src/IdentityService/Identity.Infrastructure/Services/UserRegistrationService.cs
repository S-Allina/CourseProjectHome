using AutoMapper;
using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IMainApiClient _mainApiClient;
        private readonly IUserService _userService;

        public UserRegistrationService(UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<UserService> logger, IEmailService emailService, IMainApiClient mainApiClient, IUserService userService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _mainApiClient = mainApiClient;
            _userService = userService;
        }

        public async Task<UserRegistrationResponseDto> RegisterAsync(UserRegistrationRequestDto requestDto)
        {
            _logger.LogInformation("Registering user.");

            await ValidateRequestAsync(requestDto);

            var newUser = await CreateUserAsync(requestDto);

            await _emailService.SendVerificationEmailAsync(newUser);

            await _userManager.AddToRoleAsync(newUser, Roles.User.ToString());

            var response = new UserRegistrationResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email ?? string.Empty,
                Message = "Ваш профиль создан успешно. Подтвердите пожалуйста почту для окончания регистрации."
            };

            return response;
        }

        public async Task<UserRegistrationResponseDto> RegisterManagerAsync(UserRegistrationRequestDto requestDto)
        {
            _logger.LogInformation("Registering manager.");

            await ValidateRequestAsync(requestDto);

            var newUser = await CreateUserAsync(requestDto);

            await _emailService.SendVerificationEmailAsync(newUser);

            await _userManager.AddToRoleAsync(newUser, Roles.Manager.ToString());

            var response = new UserRegistrationResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email!,
                Message = "Профиль менеджера создан успешно. Подтвердите пожалуйста почту для окончания регистрации."
            };

            return response;
        }

        public async Task ConfirmEmailAsync(string token, string email)
        {
            await _emailService.ConfirmEmailAsync(token, email);

            var user = await _userManager.FindByEmailAsync(email);

            await _userService.UpdateUsersStatusAsync(new string[] { user?.Id ?? string.Empty }, user => user.Status == Statuses.Blocked ? Statuses.Blocked : Statuses.Activity, new CancellationToken());
        }

        private async Task ValidateRequestAsync(UserRegistrationRequestDto requestDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(requestDto.Email);

            if (existingUser != null)
            {
                _logger.LogError("Email already exists.");
                throw new Exception("Email already exists.");
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(UserRegistrationRequestDto requestDto)
        {
            var newUser = _mapper.Map<ApplicationUser>(requestDto);
            newUser.UserName = GenerateUserName(newUser.FirstName, newUser.LastName);

            var result = await _userManager.CreateAsync(newUser, requestDto.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create user");

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {errors}");
            }

            _ = Task.Run(async () =>
            {
                var success = await _mainApiClient.CreateUserAsync(newUser.Id, newUser.FirstName, newUser.LastName,newUser.Email ?? string.Empty);

                if (!success)
                    _logger.LogWarning("Failed to sync user with Main API: {UserId}", newUser.Id);
            });
            return newUser;
        }

        private string GenerateUserName(string firstName, string lastName)
        {
            var baseUserName = $"{firstName}{lastName}".ToLower();

            var count = _userManager.Users.Count(u => u.UserName == baseUserName);

            baseUserName = $"{baseUserName}{count}";

            return baseUserName;
        }
    }
}
