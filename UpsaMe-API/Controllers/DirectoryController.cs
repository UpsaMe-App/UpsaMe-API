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

        /// <summary>
        /// Lista todas las facultades.
        /// </summary>
        [HttpGet("faculties")]
        public async Task<IActionResult> GetFaculties()
        {
            var faculties = await _directoryService.GetFacultiesAsync();
            return Ok(faculties);
        }

        /// <summary>
        /// Lista las carreras pertenecientes a una facultad.
        /// </summary>
        /// <param name="facultyId">GUID de la facultad.</param>
        [HttpGet("careers")]
        public async Task<IActionResult> GetCareers([FromQuery] Guid facultyId)
        {
            if (facultyId == Guid.Empty)
                return BadRequest(new { message = "El parámetro facultyId es obligatorio." });

            var careers = await _directoryService.GetCareersByFacultyAsync(facultyId);
            return Ok(careers);
        }

        /// <summary>
        /// Lista las materias (opcionalmente filtradas por carrera).
        /// </summary>
        /// <param name="careerId">GUID opcional de la carrera.</param>
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects([FromQuery] Guid? careerId = null)
        {
            var subjects = await _directoryService.GetSubjectsAsync(careerId);
            return Ok(subjects);
        }
    }
}

