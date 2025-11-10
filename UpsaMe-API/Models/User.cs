using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required, EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Career { get; set; }

        [Range(1, 12, ErrorMessage = "El semestre debe estar entre 1 y 12.")]
        public int? Semester { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Url]
        public string? ProfilePhotoUrl { get; set; }

        [MaxLength(50)]
        public string? Timezone { get; set; } = "America/La_Paz";

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
    }
}