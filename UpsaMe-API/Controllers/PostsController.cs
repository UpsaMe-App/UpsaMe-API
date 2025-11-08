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

        [HttpGet]
        public IActionResult GetFeed([FromQuery] PostRole? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var feed = _service.GetFeedAsync(role, page, pageSize);
            return Ok(feed);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Content))
                return BadRequest("El contenido no puede estar vacío.");

            var created = await _service.CreateAsync(post);
            return CreatedAtAction(nameof(GetFeed), new { id = created.Id }, created);
        }

        [HttpPost("{id}/replies")]
        public async Task<IActionResult> AddReply(Guid id, [FromBody] PostReply reply)
        {
            if (string.IsNullOrWhiteSpace(reply.Content))
                return BadRequest("El contenido no puede estar vacío.");

            var created = await _service.AddReplyAsync(id, reply);
            return Ok(created);
        }
    }
}