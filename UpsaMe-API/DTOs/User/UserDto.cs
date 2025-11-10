using System;

namespace UpsaMe_API.DTOs.User
{
    /// <summary>
    /// Representa la información pública del usuario que se devuelve al cliente.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Correo institucional del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo del usuario (nombre + apellido).
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Carrera universitaria del usuario (opcional).
        /// </summary>
        public string? Career { get; set; }

        /// <summary>
        /// Semestre actual del usuario (opcional).
        /// </summary>
        public int? Semester { get; set; }

        /// <summary>
        /// URL pública de la foto de perfil (en Azure Blob Storage).
        /// </summary>
        public string? ProfilePhotoUrl { get; set; }
    }
}