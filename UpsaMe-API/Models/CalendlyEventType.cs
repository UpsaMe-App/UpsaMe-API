using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    [Table("CalendlyEventTypes")]
    public class CalendlyEventType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Usuario al que pertenece este tipo de evento (referencia externa).
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// URI único del tipo de evento en Calendly.
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string EventTypeUri { get; set; } = string.Empty;

        /// <summary>
        /// Nombre descriptivo del tipo de evento.
        /// </summary>
        [MaxLength(200)]
        public string? Name { get; set; }

        /// <summary>
        /// Duración del evento en minutos.
        /// </summary>
        public int DurationMin { get; set; }

        /// <summary>
        /// Indica si el evento está activo y sincronizado actualmente.
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Fecha y hora (UTC) en que se sincronizó por última vez.
        /// </summary>
        public DateTime SyncedAtUtc { get; set; } = DateTime.UtcNow;
    }
}