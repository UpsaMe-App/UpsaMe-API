using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Code { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        [Required]
        [ForeignKey(nameof(Career))]
        public Guid CareerId { get; set; }
        public Career? Career { get; set; }
    }
}