using System.ComponentModel.DataAnnotations;

namespace UpsaMe_API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? Career { get; set; }
        public int? Semester { get; set; }

        public string? Phone { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? Timezone { get; set; } = "America/La_Paz";

        public string? PasswordHash { get; set; }
    }
}