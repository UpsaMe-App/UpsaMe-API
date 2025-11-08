using System.ComponentModel.DataAnnotations;

namespace UpsaMe_API.Models
{
    public class CalendlyEventType
    {
        [Key]
        public int Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        public string EventTypeUri { get; set; } = string.Empty;

        public string? Name { get; set; }
        public int DurationMin { get; set; }
        public bool Active { get; set; } = true;
        public DateTime SyncedAtUtc { get; set; } = DateTime.UtcNow;
    }
}