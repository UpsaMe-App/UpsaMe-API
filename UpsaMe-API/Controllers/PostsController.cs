using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpsaMe_API.Models;
using UpsaMe_API.Services;

namespace UpsaMe_API.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostsController : ControllerBase
    {
        private readonly PostService _service;

        public PostsController(PostService service)
        {
            _service = service;
        }

        /// <summary>Obtiene el feed de publicaciones.</summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeed(
            [FromQuery] PostRole? role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var feed = await _service.GetFeedAsync(role, page, pageSize);
            return Ok(feed);
        }

        /// <summary>Crea una nueva publicación.</summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Post), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Content))
                return BadRequest("El contenido no puede estar vacío.");

            var created = await _service.CreateAsync(post);
            return CreatedAtAction(nameof(GetFeed), new { id = created.Id }, created);
        }

        /// <summary>Agrega una respuesta (reply) a una publicación.</summary>
        [HttpPost("{id}/replies")]
        [Authorize]
        [ProducesResponseType(typeof(PostReply), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddReply(Guid id, [FromBody] PostReply reply)
        {
            if (string.IsNullOrWhiteSpace(reply.Content))
                return BadRequest("El contenido no puede estar vacío.");

            var created = await _service.AddReplyAsync(id, reply);
            if (created == null)
                return NotFound("No se encontró la publicación.");

            return Ok(created);
        }
    }
}