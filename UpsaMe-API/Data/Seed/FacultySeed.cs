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