using Microsoft.AspNetCore.Mvc;
using System.IO;

[ApiController]
[Route("users")]
public class AvatarsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public AvatarsController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet("avatars")]
    public IActionResult GetAvatars()
    {
        var wwwroot = _env.WebRootPath; // path a wwwroot
        var avatarsDir = Path.Combine(wwwroot ?? string.Empty, "avatars");
        if (!Directory.Exists(avatarsDir))
            return Ok(new object[0]);

        var files = Directory.GetFiles(avatarsDir)
            .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            .Select(f => {
                var name = Path.GetFileName(f);
                var url = $"{Request.Scheme}://{Request.Host}/avatars/{name}";
                return new { id = name, name = name, url = url };
            }).ToArray();

        return Ok(files);
    }
}