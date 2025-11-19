using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UpsaMe_API.Models
{
    public enum PostRole { Helper = 1, Student = 2, Comment = 3 }
    public enum PostStatus { Active, Closed, Deleted }

    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public PostRole Role { get; set; }

        // Para ayudante: Objetivo de la ayudantía
        // Para estudiante: lo podés usar como título libre, o dejar null
        public string? Title { get; set; }

        // Para ayudante: descripción / contenido
        // Para estudiante: descripción adicional si quieres
        [Required]
        public string Content { get; set; } = string.Empty;

        [ForeignKey("Subject")]
        public Guid? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        // Cantidad de personas:
        //   - Helper: capacidad máxima de ayudantía
        //   - Student: cantidad de personas que busca
        public int? Capacity { get; set; }

        public int CapacityUsed { get; set; } = 0;
        public PostStatus Status { get; set; } = PostStatus.Active;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        // 🔹 NUEVO: nombre del docente (para rol Helper)
        [MaxLength(200)]
        public string? TeacherName { get; set; }

        // 🔹 NUEVO: temas (para Helper o Student)
        // Guardamos como texto plano tipo: "Límites, Derivadas, Integrales"
        [MaxLength(500)]
        public string? Topics { get; set; }

        public ICollection<PostReply>? Replies { get; set; }
    }
}