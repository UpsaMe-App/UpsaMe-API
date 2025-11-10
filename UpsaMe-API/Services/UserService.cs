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

        public async Task<UserDto?> GetProfileAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

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

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken ct = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            // Actualizaciones básicas
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.LastName))  user.LastName  = dto.LastName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Phone))     user.Phone     = dto.Phone.Trim();
            if (dto.Semester.HasValue)                     user.Semester  = dto.Semester.Value;

            // Foto de perfil (opcional)
            if (dto.ProfilePhoto != null && dto.ProfilePhoto.Length > 0)
            {
                var contentType = string.IsNullOrWhiteSpace(dto.ProfilePhoto.ContentType)
                    ? "image/jpeg"
                    : dto.ProfilePhoto.ContentType;

                using var stream = dto.ProfilePhoto.OpenReadStream();
                user.ProfilePhotoUrl = await _blobStorageHelper
                    .UploadProfilePhotoAsync(user.Id, stream, contentType);
            }

            // No hace falta _context.Users.Update(user) si la entidad está siendo trackeada
            await _context.SaveChangesAsync(ct);
        }
    }
}
