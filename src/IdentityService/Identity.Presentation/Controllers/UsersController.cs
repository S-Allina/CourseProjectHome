using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPatch("block")]
        [Authorize]
        public async Task<ResponseDto> Block([FromBody] string[] userIds, CancellationToken cancellationToken)
        {
            return await _userService.BlockUser(userIds, cancellationToken);
        }

        [HttpPatch("role")]
        [Authorize]
        public async Task<ResponseDto> RoleChange([FromBody] ChangeRoleRequest request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUsersRoleAsync(request.UserIds, request.Role, cancellationToken);
        }

        [HttpPatch("unblock")]
        [Authorize]
        public async Task<ResponseDto> Unblock([FromBody] string[] userIds, CancellationToken cancellationToken)
        {
            return await _userService.UnlockUser(userIds, cancellationToken);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDto> Get(CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }

        [HttpDelete("unconfirmedUsers")]
        [Authorize]
        public async Task<ResponseDto> DeleteUnconfirmedUsers(CancellationToken cancellationToken)
        {
            return await _userService.DeleteUnconfirmedUsersAsync(cancellationToken);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ResponseDto> DeleteUsers([FromBody] string[] userIds, CancellationToken cancellationToken)
        {
            return await _userService.DeleteSomeUsersAsync(userIds, cancellationToken);
        }
    }
}