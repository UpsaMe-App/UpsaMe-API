using UpsaMe_API.Models;

namespace UpsaMe_API.Data.Seed
{
    public static class CareerSeed
    {
        public static List<Career> GetCareers(List<Faculty> faculties)
        {
            var engineering = faculties.First(f => f.Slug == "ingenieria");
            var business = faculties.First(f => f.Slug == "empresariales");
            var law = faculties.First(f => f.Slug == "juridicas");

            return new List<Career>
            {
                // Ingeniería
                new Career
                {
                    Id = Guid.NewGuid(),
                    Name = "Ingeniería de Software",
                    Slug = "ing-software",
                    FacultyId = engineering.Id
                },
                new Career
                {
                    Id = Guid.NewGuid(),
                    Name = "Ingeniería Industrial",
                    Slug = "ing-industrial",
                    FacultyId = engineering.Id
                },

                // Empresariales
                new Career
                {
                    Id = Guid.NewGuid(),
                    Name = "Administración de Empresas",
                    Slug = "adm-empresas",
                    FacultyId = business.Id
                },
                new Career
                {
                    Id = Guid.NewGuid(),
                    Name = "Marketing y Publicidad",
                    Slug = "marketing",
                    FacultyId = business.Id
                },

                // Jurídicas
                new Career
                {
                    Id = Guid.NewGuid(),
                    Name = "Derecho Empresarial",
                    Slug = "derecho-empresarial",
                    FacultyId = law.Id
                }
            };
        }

        public static void Seed(UpsaMeDbContext db)
        {
            if (!db.Careers.Any())
            {
                var faculties = db.Faculties.ToList();
                var careers = GetCareers(faculties);
                db.Careers.AddRange(careers);
                db.SaveChanges();
            }
        }
    }
}
