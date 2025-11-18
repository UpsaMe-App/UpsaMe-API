using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpsaMe_API.DTOs.Notifications;
using UpsaMe_API.Services;
using System.Security.Claims;
using UpsaMe_API.Data;
using UpsaMe_API.Models;

[ApiController]
[Route("notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _svc;
    private readonly UpsaMeDbContext _db;

    public NotificationsController(NotificationService svc, UpsaMeDbContext db)
    {
        _svc = svc;
        _db = db;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim == null ? Guid.Empty : Guid.Parse(claim.Value);
    }

    [HttpPost("devices")]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var existing = _db.NotificationDevices.FirstOrDefault(d => d.UserId == userId && d.Provider == dto.Provider && d.DeviceId == dto.DeviceId);
        if (existing == null)
        {
            _db.NotificationDevices.Add(new NotificationDevice { Id = Guid.NewGuid(), UserId = userId, Provider = dto.Provider, DeviceId = dto.DeviceId, PushToken = dto.PushToken, LastSeenAtUtc = DateTime.UtcNow });
            await _db.SaveChangesAsync();
        }
        else
        {
            existing.LastSeenAtUtc = DateTime.UtcNow;
            existing.PushToken = dto.PushToken ?? existing.PushToken;
            await _db.SaveChangesAsync();
        }
        return Ok();
    }

    [HttpGet]
    public IActionResult List([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        var q = _svc.QueryForUser(userId).Skip((page - 1) * pageSize).Take(pageSize)
                .Select(n => new NotificationDto { Id = n.Id, Title = n.Title, Body = n.Body, DataJson = n.DataJson, IsRead = n.IsRead, CreatedAtUtc = n.CreatedAtUtc });
        return Ok(q.ToList());
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        await _svc.MarkReadAsync(id);
        return Ok();
    }

    // Only for admin/testing - send a test push to current user
    [HttpPost("send-test")]
    public async Task<IActionResult> SendTest([FromBody] string message)
    {
        var userId = GetUserId();
        var n = await _svc.CreateAsync(userId, "Test", message);
        return Ok(n);
    }
}