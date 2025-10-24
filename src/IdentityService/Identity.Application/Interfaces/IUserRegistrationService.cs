using Identity.Application.Dto;

namespace Identity.Application.Interfaces
{
    public interface IUserRegistrationService
    {
        Task<UserRegistrationResponseDto> RegisterAsync(UserRegistrationRequestDto requestDto);
        Task<UserRegistrationResponseDto> RegisterManagerAsync(UserRegistrationRequestDto requestDto);
        Task ConfirmEmailAsync(string token, string email);
    }
}
