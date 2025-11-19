using System.ComponentModel.DataAnnotations;

namespace UpsaMe_API.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string LastName { get; set; } = string.Empty;

        // 🔹 Ahora por Id
        public Guid? CareerId { get; set; }

        [Range(1, 10, ErrorMessage = "El semestre debe estar entre 1 y 12.")]
        public int? Semester { get; set; }
    }
}