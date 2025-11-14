using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpsaMe_API.Services;

namespace UpsaMe_API.Controllers
{
    [ApiController]
    [Route("directory")]
    public class DirectoryController : ControllerBase
    {
        private readonly DirectoryService _directoryService;

        public DirectoryController(DirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        // ================================================================
        // 📌 1. FACULTADES (acordeón principal)
        // ================================================================
        [HttpGet("faculties")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFaculties()
        {
            try
            {
                var faculties = await _directoryService.GetFacultiesAsync();
                return Ok(faculties);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Error obteniendo facultades",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // ================================================================
        // 📌 2. CARRERAS POR FACULTAD (acordeón interno)
        // ================================================================
        [HttpGet("faculties/{facultyId:guid}/careers")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCareersByFaculty(Guid facultyId)
        {
            if (facultyId == Guid.Empty)
                return Problem(
                    title: "Parámetro inválido",
                    detail: "El parámetro facultyId es obligatorio.",
                    statusCode: StatusCodes.Status400BadRequest
                );

            try
            {
                var careers = await _directoryService.GetCareersByFacultyAsync(facultyId);
                return Ok(careers);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Error obteniendo carreras",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // ================================================================
        // 📌 3. LISTA DE USUARIOS POR CARRERA (cards)
        // ================================================================
        [HttpGet("careers/{careerId:guid}/users")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersByCareer(Guid careerId)
        {
            try
            {
                var users = await _directoryService.GetUsersByCareerAsync(careerId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Error obteniendo usuarios de la carrera",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // ================================================================
        // 📌 4. BUSCADOR DE MATERIAS (para la lupita)
        // ================================================================
        [HttpGet("subjects/search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchSubjects([FromQuery] string q)
        {
            try
            {
                var subjects = await _directoryService.SearchSubjectsAsync(q);
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Error buscando materias",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // ================================================================
        // 📌 5. LISTAR MATERIAS (opcionalmente filtradas por carrera + paginación)
        // ================================================================
        [HttpGet("subjects")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubjects(
            [FromQuery] Guid? careerId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return Problem(
                    title: "Parámetros de paginación inválidos",
                    detail: "Usa page >= 1 y pageSize entre 1 y 100.",
                    statusCode: StatusCodes.Status400BadRequest
                );

            try
            {
                var subjects = await _directoryService.GetSubjectsAsync(careerId);

                var total = subjects.Count();
                var items = subjects
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                return Ok(new
                {
                    page,
                    pageSize,
                    total,
                    items
                });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Error obteniendo materias",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
