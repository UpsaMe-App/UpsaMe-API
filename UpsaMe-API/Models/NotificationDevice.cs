using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    [Table("NotificationDevices")]
    public class NotificationDevice
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, MaxLength(50)]
        public string Provider { get; set; } = string.Empty; // e.g., "OneSignal"

        // Provider-specific device identifier (OneSignal player id)
        [Required, MaxLength(200)]
        public string DeviceId { get; set; } = string.Empty;

        // Optional push token if provider has a separate token
        [MaxLength(500)]
        public string? PushToken { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeenAtUtc { get; set; }
    }
}