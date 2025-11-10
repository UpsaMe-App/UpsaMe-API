using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    [Table("CalendlyEvents")]
    public class CalendlyEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// URI único del evento en Calendly.
        /// </summary>
        [Required]
        [MaxLength(450)] // evita índices largos si EF lo crea
        public string EventUri { get; set; } = string.Empty;

        /// <summary>
        /// URI del invitado (si aplica).
        /// </summary>
        public string? InviteeUri { get; set; }

        /// <summary>
        /// Fecha y hora de inicio del evento (UTC).
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Fecha y hora de finalización del evento (UTC).
        /// </summary>
        public DateTime EndUtc { get; set; }

        /// <summary>
        /// Estado actual del evento (ej. "active", "canceled").
        /// </summary>
        [MaxLength(100)]
        public string? Status { get; set; }

        /// <summary>
        /// JSON crudo de Calendly (puede contener metadatos adicionales).
        /// </summary>
        public string? RawJson { get; set; }
    }
}