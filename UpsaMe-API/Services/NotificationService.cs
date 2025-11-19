using System.Text.Json;
using UpsaMe_API.Data;
using UpsaMe_API.Helpers;
using UpsaMe_API.Models;

namespace UpsaMe_API.Services;

public class NotificationService
{
    private readonly UpsaMeDbContext _db;
    private readonly OneSignalHelper _oneSignal;

    public NotificationService(UpsaMeDbContext db, OneSignalHelper oneSignal)
    {
        _db = db;
        _oneSignal = oneSignal;
    }

    public async Task<Notification> CreateAsync(Guid userId, string title, string body, object? data = null)
    {
        var n = new Notification { Id = Guid.NewGuid(), UserId = userId, Title = title, Body = body, DataJson = data == null ? null : JsonSerializer.Serialize(data) };
        _db.Notifications.Add(n);
        await _db.SaveChangesAsync();

        // Send push if device exists (send to all devices)
        var devices = _db.NotificationDevices.Where(d => d.UserId == userId).ToList();
        foreach (var d in devices)
        {
            try
            {
                if (d.Provider == "OneSignal")
                {
                    await _oneSignal.SendToDeviceAsync(d.DeviceId, title, body, data);
                }
            }
            catch
            {
                // log and continue
            }
        }

        return n;
    }

    public IQueryable<Notification> QueryForUser(Guid userId) => _db.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAtUtc);

    public async Task MarkReadAsync(Guid id)
    {
        var n = await _db.Notifications.FindAsync(id);
        if (n != null) { n.IsRead = true; await _db.SaveChangesAsync(); }
    }
}