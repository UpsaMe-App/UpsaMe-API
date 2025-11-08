namespace UpsaMe_API.DTOs.Directory
{
    public class CareerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int FacultyId { get; set; }
    }
}