namespace UpsaMe_API.DTOs.Notifications; 
public class RegisterDeviceDto
{
    public string Provider { get; set; } = "OneSignal";
    public string DeviceId { get; set; } = string.Empty; // OneSignal player id
    public string? PushToken { get; set; } // optional
}