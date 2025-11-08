using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }
        public string? Slug { get; set; }

        public Guid CareerId { get; set; }
        public Career? Career { get; set; }
    }
}