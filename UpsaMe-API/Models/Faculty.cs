using System.ComponentModel.DataAnnotations;

namespace UpsaMe_API.Models
{
    public class Faculty
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;

        public ICollection<Career>? Careers { get; set; }
    }
}