using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    [Table("Careers")]
    public class Career
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public Guid FacultyId { get; set; }
        public Faculty? Faculty { get; set; }

        public ICollection<Subject>? Subjects { get; set; }

        // 🔹 Nuevo: usuarios en esta carrera
        public ICollection<User>? Users { get; set; }
    }
}