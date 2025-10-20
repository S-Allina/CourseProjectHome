using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Users.Application.Dto;

namespace Identity.Presentation.Controllers
{
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await _userService.GetCurrentUserAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _userService.GetByIdAsync(id);

            return Ok(response);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var response = await _userService.GetAllAsync(cancellationToken);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequestDto requestDto)
        {
            var result = await _userService.UpdateAsync(id, requestDto);

            return Ok(result);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ResponseDto> DeleteUsers([FromBody] string[] userIds, CancellationToken cancellationToken)
        {
            return await _userService.DeleteSomeUsersAsync(userIds, cancellationToken);
        }
    }
}