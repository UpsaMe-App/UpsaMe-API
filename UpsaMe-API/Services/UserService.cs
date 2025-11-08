using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Data;
using UpsaMe_API.DTOs.User;
using UpsaMe_API.Helpers;

namespace UpsaMe_API.Services
{
    public class UserService
    {
        private readonly UpsaMeDbContext _context;
        private readonly BlobStorageHelper _blobStorageHelper;

        public UserService(UpsaMeDbContext context, BlobStorageHelper blobStorageHelper)
        {
            _context = context;
            _blobStorageHelper = blobStorageHelper;
        }

        public async Task<UserDto?> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Career = user.Career,
                Semester = user.Semester,
                ProfilePhotoUrl = user.ProfilePhotoUrl
            };
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("Usuario no encontrado.");

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Semester = dto.Semester ?? user.Semester;

            if (dto.ProfilePhoto != null)
            {
                using var stream = dto.ProfilePhoto.OpenReadStream();
                user.ProfilePhotoUrl = await _blobStorageHelper.UploadProfilePhotoAsync(
                    user.Id, stream, dto.ProfilePhoto.ContentType);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}