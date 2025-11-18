using Microsoft.AspNetCore.Mvc;

namespace UpsaMe_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendlyWebhookController : ControllerBase
{
    // POST api/calendly/webhook
    [HttpPost("webhook")]
    public IActionResult Receive([FromBody] object payload)
    {
        // TODO: validar cabeceras / firma según Calendly docs
        // Procesar: leer tipo de evento y notificar a usuarios / crear DB rows
        return Ok();
    }
}