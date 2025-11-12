using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        /// <summary>Obtiene el perfil del usuario autenticado.</summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
                return Unauthorized("Token inválido: no se encontró el ID de usuario.");

            var userId = Guid.Parse(userIdClaim.Value);
            var user = await _userService.GetProfileAsync(userId);
            return Ok(user);
        }

        /// <summary>Actualiza el perfil del usuario autenticado (nombre, teléfono, semestre, foto).</summary>
        [HttpPut]
        [RequestSizeLimit(10_000_000)] // Límite 10MB por si suben fotos
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
                return Unauthorized("Token inválido: no se encontró el ID de usuario.");

            var userId = Guid.Parse(userIdClaim.Value);

            // ✅ Actualiza el perfil
            await _userService.UpdateProfileAsync(userId, dto);

            // ✅ Obtiene el perfil actualizado
            var updatedProfile = await _userService.GetProfileAsync(userId);
            if (updatedProfile == null)
                return NotFound("No se pudo recuperar el perfil actualizado.");

            // ✅ Devuelve el perfil actualizado
            return Ok(updatedProfile);
        }

    }
}