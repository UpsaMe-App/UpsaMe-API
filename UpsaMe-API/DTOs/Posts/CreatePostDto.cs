namespace UpsaMe_API.DTOs.Posts
{
    public class CreatePostDto
    {
        // 1=Helper, 2=Student, 3=Comment
        public UpsaMe_API.Models.PostRole Role { get; set; }

        // Helper: Objetivo de la ayudantía
        // Student: opcional
        public string? Title { get; set; }

        // Helper: descripción / contenido
        // Student: puede ser una explicación extra
        public string Content { get; set; } = string.Empty;

        // Materia (rol Helper y Student)
        public Guid? SubjectId { get; set; }

        // Helper: capacidad máxima
        // Student: cantidad de personas que quiere en la sesión
        public int? Capacity { get; set; }

        // Helper: nombre del docente
        public string? TeacherName { get; set; }

        // Student: Temas
        public string[]? Topics { get; set; }
    }
}