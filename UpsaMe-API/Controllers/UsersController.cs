using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpsaMe_API.DTOs.User;
using UpsaMe_API.Services;

namespace UpsaMe_API.Controllers
{
    [ApiController]
    [Route("me")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("nameidentifier")!.Value);
            var user = await _userService.GetProfileAsync(userId);
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("nameidentifier")!.Value);
            await _userService.UpdateProfileAsync(userId, dto);
            return NoContent();
        }
    }
}