using System.ComponentModel.DataAnnotations;

namespace UpsaMe_API.Models
{
    public class CalendlyEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EventUri { get; set; } = string.Empty;

        public string? InviteeUri { get; set; }

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string? Status { get; set; }
        public string? RawJson { get; set; }
    }
}