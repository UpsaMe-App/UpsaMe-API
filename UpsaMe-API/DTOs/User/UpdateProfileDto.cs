namespace UpsaMe_API.DTOs.User
{
    public class UpdateProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public int? Semester { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }
}