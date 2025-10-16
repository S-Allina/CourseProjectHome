using Identity.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IUserRegistrationService
    {
        Task<UserRegistrationResponseDto> RegisterAsync(UserRegistrationRequestDto requestDto);
        Task<UserRegistrationResponseDto> RegisterManagerAsync(UserRegistrationRequestDto requestDto);
        Task ConfirmEmailAsync(string token, string email);
    }
}
