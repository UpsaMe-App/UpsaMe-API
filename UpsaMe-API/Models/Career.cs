using System.ComponentModel.DataAnnotations;

namespace UpsaMe_API.Models
{
    public class Career
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;

        public Guid FacultyId { get; set; }
        public Faculty? Faculty { get; set; }

        public ICollection<Subject>? Subjects { get; set; }
    }
}