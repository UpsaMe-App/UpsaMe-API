namespace UpsaMe_API.DTOs.Directory
{
    public class CareerDto
    {
        public Guid Id { get; set; }        // ← cambia int → Guid
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Guid FacultyId { get; set; } // ← cambia int → Guid
    }
}