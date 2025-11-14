using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UpsaMe_API.DTOs.User;
using UpsaMe_API.Services;

namespace UpsaMe_API.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize] // 👈 por defecto todos necesitan auth, salvo donde pongamos [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>Perfil del usuario autenticado.</summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
                return Unauthorized("Token inválido: no se encontró el ID de usuario.");

            var userId = Guid.Parse(userIdClaim.Value);
            var user = await _userService.GetProfileAsync(userId);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            return Ok(user);
        }

        /// <summary>
        /// Perfil público por Id (para cuando clickean el nombre en un post).
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous] // 👈 cualquiera puede ver el perfil público
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicProfile(Guid id)
        {
            var user = await _userService.GetProfileAsync(id);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            return Ok(user);
        }

        /// <summary>
        /// Actualiza el perfil del usuario autenticado (nombre, teléfono, semestre, foto).
        /// </summary>
        [HttpPut("me")]
        [RequestSizeLimit(10_000_000)] // 10MB para la foto
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
                return Unauthorized("Token inválido: no se encontró el ID de usuario.");

            var userId = Guid.Parse(userIdClaim.Value);

            await _userService.UpdateProfileAsync(userId, dto);

            var updatedProfile = await _userService.GetProfileAsync(userId);
            if (updatedProfile == null)
                return NotFound("No se pudo recuperar el perfil actualizado.");

            return Ok(updatedProfile);
        }
    }
}
