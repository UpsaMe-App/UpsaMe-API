namespace UpsaMe_API.DTOs.Directorys
{
    public class UserCardDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Career { get; set; }
        public int? Semester { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }
}