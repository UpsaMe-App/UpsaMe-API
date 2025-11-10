using System.ComponentModel.DataAnnotations;

namespace UpsaMe_API.Models
{
    public class WebhookLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Source { get; set; } = string.Empty;

        [Required]
        public string Payload { get; set; } = string.Empty;

        public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    }
}