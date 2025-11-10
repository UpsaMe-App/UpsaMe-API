using UpsaMe_API.Models;

namespace UpsaMe_API.Data.Seed
{
    public static class FacultySeed
    {
        public static List<Faculty> GetFaculties()
        {
            return new List<Faculty>
            {
                new Faculty
                {
                    Id = Guid.NewGuid(),
                    Name = "Facultad de Ingeniería",
                    Slug = "ingenieria"
                },
                new Faculty
                {
                    Id = Guid.NewGuid(),
                    Name = "Facultad de Arquitectura, diseño y urbanismo",
                    Slug = "arquitectura"
                },
                new Faculty
                {
                    Id = Guid.NewGuid(),
                    Name = "Facultad de Humanidades, Comunicacion y Artes",
                    Slug = "humanidades"
                },
                new Faculty
                {
                    Id = Guid.NewGuid(),
                    Name = "Facultad de Ciencias Empresariales",
                    Slug = "empresariales"
                },
                new Faculty
                {
                    Id = Guid.NewGuid(),
                    Name = "Facultad de Ciencias Jurídicas",
                    Slug = "juridicas"
                }
            };
        }
        public static void Upsert(UpsaMeDbContext db)
        {
            var desired = GetFaculties();
            var existingSlugs = db.Faculties.Select(f => f.Slug).ToHashSet();

            var toAdd = desired.Where(f => !existingSlugs.Contains(f.Slug)).ToList();
            if (toAdd.Count > 0)
            {
                db.Faculties.AddRange(toAdd);
                db.SaveChanges();
            }
        }
        public static void Seed(UpsaMeDbContext db)
        {
            if (!db.Faculties.Any())
            {
                var faculties = GetFaculties();
                db.Faculties.AddRange(faculties);
                db.SaveChanges();
            }
        }
    }
}