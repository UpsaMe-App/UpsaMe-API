using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    public enum PostRole { Helper = 1, Student = 2, Comment = 3 }
    public enum PostStatus { Active, Closed, Deleted }

    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public PostRole Role { get; set; }

        public string? Title { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;

        [ForeignKey("Subject")]
        public Guid? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? Capacity { get; set; }
        public int CapacityUsed { get; set; } = 0;
        public PostStatus Status { get; set; } = PostStatus.Active;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<PostReply>? Replies { get; set; }
    }
}

