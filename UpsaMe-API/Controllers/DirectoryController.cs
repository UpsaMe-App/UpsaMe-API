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

        /// <summary>Lista todas las facultades.</summary>
        [HttpGet("faculties")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFaculties(CancellationToken ct)
        {
            try
            {
                var faculties = await _directoryService.GetFacultiesAsync();
                return Ok(faculties);
            }
            catch (Exception ex)
            {
                return Problem(title: "Error obteniendo facultades", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Lista las carreras de una facultad.</summary>
        /// <param name="facultyId">GUID de la facultad.</param>
        [HttpGet("careers")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCareers([FromQuery] Guid facultyId, CancellationToken ct)
        {
            if (facultyId == Guid.Empty)
                return Problem(title: "Parámetro inválido", detail: "El parámetro facultyId es obligatorio.", statusCode: StatusCodes.Status400BadRequest);

            try
            {
                var careers = await _directoryService.GetCareersByFacultyAsync(facultyId);
                return Ok(careers);
            }
            catch (Exception ex)
            {
                return Problem(title: "Error obteniendo carreras", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Lista materias (opcionalmente filtradas por carrera). Soporta paginación simple.</summary>
        /// <param name="careerId">GUID opcional de la carrera.</param>
        /// <param name="page">Página (>=1). Default 1.</param>
        /// <param name="pageSize">Tamaño de página (1–100). Default 50.</param>
        [HttpGet("subjects")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSubjects(
            [FromQuery] Guid? careerId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return Problem(title: "Parámetros de paginación inválidos", detail: "Usa page >= 1 y pageSize entre 1 y 100.", statusCode: StatusCodes.Status400BadRequest);

            try
            {
                // Si aún no implementaste paginación en el servicio, esto devolverá todo.
                // Puedes mover la paginación al servicio cuando quieras.
                var subjects = await _directoryService.GetSubjectsAsync(careerId);

                // Paginación en memoria (simple). Idealmente hazla en la query del servicio.
                var total = subjects.Count();
                var items = subjects.Skip((page - 1) * pageSize).Take(pageSize);

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
                return Problem(title: "Error obteniendo materias", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}