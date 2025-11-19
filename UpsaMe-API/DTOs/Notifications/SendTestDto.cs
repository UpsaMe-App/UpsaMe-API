namespace UpsaMe_API.DTOs.Notifications
{
    public class SendTestDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public object? Data { get; set; }
    }
}