using System.ComponentModel.DataAnnotations;
using UpsaMe_API.Models.Enums;

namespace UpsaMe_API.Models
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MentorUserId { get; set; }
        public User? Mentoruser { get; set;  }
        
        [Required, EmailAddress]
        public string InviteeEmail { get; set; } = string.Empty;

        [Required]
        public DateTime StartsAtUtc { get; set; }
        
        [Required]
        public DateTime EndsAtUtc { get; set; }
        
        public string? MeetingUrl { get; set; }

        [Required]
        public SessionStatus Status { get; set; } = SessionStatus.Scheduled;

        public string? DealId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}