using Identity.Application.Dto;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPatch("status")]
        [Authorize]
        public async Task<ResponseDto> StatusChange([FromBody] ChangeStatusRequest request,  CancellationToken cancellationToken)
        {
            return await _userService.StatusChangeAsync(request.UserIds, request.Status, cancellationToken);
        }

        [HttpPatch("role")]
        [Authorize]
        public async Task<ResponseDto> RoleChange([FromBody] ChangeRoleRequest request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUsersRoleAsync(request.UserIds, request.Role, cancellationToken);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDto> Get(CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }

        [HttpGet("check-block")]
        [Authorize]
        public async Task<ResponseDto> CheckBlock(string id, CancellationToken cancellationToken)
        {
            return await _userService.CheckBlockAsync(id, cancellationToken);
        }

        [HttpDelete("unconfirmed")]
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