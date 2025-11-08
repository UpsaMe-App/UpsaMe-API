using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Data;
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

        // Facultades
        public async Task<IEnumerable<Faculty>> GetFacultiesAsync()
        {
            return await _context.Faculties
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        // Carreras por facultad
        public async Task<IEnumerable<object>> GetCareersByFacultyAsync(Guid facultyId)
        {
            return await _context.Careers
                .Where(c => c.FacultyId == facultyId)
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug
                })
                .ToListAsync();
        }

        // Materias (por carrera opcional)
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
    }
}