using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Data;
using UpsaMe_API.DTOs.Directory;
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
    }
}