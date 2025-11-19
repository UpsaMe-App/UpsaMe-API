using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Data;
using UpsaMe_API.DTOs.Directory;
using UpsaMe_API.DTOs.Directorys; // UserCardDto
using UpsaMe_API.Models;

namespace UpsaMe_API.Services
{
    public class DirectoryService
    {
        private readonly UpsaMeDbContext _context;

        public DirectoryService(UpsaMeDbContext context)
        {
            _context = context;
        }

        // ====== FACULTADES ======
        public async Task<IEnumerable<Faculty>> GetFacultiesAsync()
        {
            return await _context.Faculties
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        // ====== CARRERAS POR FACULTAD ======
        public async Task<IEnumerable<CareerDto>> GetCareersByFacultyAsync(Guid facultyId)
        {
            return await _context.Careers
                .Where(c => c.FacultyId == facultyId)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CareerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    FacultyId = c.FacultyId
                })
                .ToListAsync();
        }

        // ====== MATERIAS (OPCIONALMENTE POR CARRERA) ======
        public async Task<IEnumerable<object>> GetSubjectsAsync(Guid? careerId = null)
        {
            var query = _context.Subjects.AsQueryable();

            if (careerId.HasValue)
                query = query.Where(s => s.CareerId == careerId.Value);

            return await query
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Slug,
                    s.CareerId
                })
                .ToListAsync();
        }

        // 🔍 Buscar materias por texto (para la lupita)
        public async Task<IEnumerable<object>> SearchSubjectsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Array.Empty<object>();

            var q = query.Trim().ToLower();

            return await _context.Subjects
                .AsNoTracking()
                .Where(s =>
                    s.Name.ToLower().Contains(q) ||
                    (s.Slug != null && s.Slug.ToLower().Contains(q)) ||
                    (s.Code != null && s.Code.ToLower().Contains(q)))
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Slug,
                    s.CareerId
                })
                .ToListAsync();
        }

        // 👥 Usuarios registrados en una carrera (por CareerId)
        public async Task<IEnumerable<UserCardDto>> GetUsersByCareerAsync(Guid careerId)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Career)
                .Where(u => u.CareerId == careerId)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UserCardDto
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    CareerId = u.CareerId,
                    Career = u.Career != null ? u.Career.Name : null,
                    Semester = u.Semester,
                    ProfilePhotoUrl = u.ProfilePhotoUrl
                })
                .ToListAsync();
        }
    }
}
