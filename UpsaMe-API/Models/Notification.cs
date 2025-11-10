using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UpsaMe_API.Models.Enums;

namespace UpsaMe_API.Models
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required, MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Body { get; set; } = string.Empty;

        public string? DataJson { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}