using System.ComponentModel.DataAnnotations;
using UpsaMe_API.Models.Enums;

namespace UpsaMe_API.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public NotificationType Type { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public string? DataJson { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}