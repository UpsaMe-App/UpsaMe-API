using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Models;

namespace UpsaMe_API.Data.Seed
{
    public static class SubjectSeed
    {
        public static List<Subject> GetSubjects(UpsaMeDbContext db)
        {
            var careers = db.Careers.AsNoTracking().ToList();

            // Buscar carreras
            var ingSoftware = careers.FirstOrDefault(c => c.Slug == "ing-software");
            var ingIndustrial = careers.FirstOrDefault(c => c.Slug == "ing-industrial");
            var adminEmpresas = careers.FirstOrDefault(c => c.Slug == "adm-empresas");
            var marketing = careers.FirstOrDefault(c => c.Slug == "marketing");
            var derechoEmp = careers.FirstOrDefault(c => c.Slug == "derecho-empresarial");

            if (ingSoftware == null || ingIndustrial == null || adminEmpresas == null || marketing == null || derechoEmp == null)
                throw new InvalidOperationException("❌ Faltan carreras requeridas para generar materias.");

            return new List<Subject>
            {
                // Ingeniería de Software
                new Subject { Id = Guid.NewGuid(), Name = "Programación I", Slug = "programacion-i", Code = "ING101", CareerId = ingSoftware.Id },
                new Subject { Id = Guid.NewGuid(), Name = "Estructura de Datos", Slug = "estructura-de-datos", Code = "ING202", CareerId = ingSoftware.Id },
                new Subject { Id = Guid.NewGuid(), Name = "Bases de Datos I", Slug = "bases-de-datos", Code = "ING303", CareerId = ingSoftware.Id },

                // Ingeniería Industrial
                new Subject { Id = Guid.NewGuid(), Name = "Gestión de Producción", Slug = "gestion-de-produccion", Code = "IND201", CareerId = ingIndustrial.Id },
                new Subject { Id = Guid.NewGuid(), Name = "Control de Calidad", Slug = "control-de-calidad", Code = "IND302", CareerId = ingIndustrial.Id },

                // Administración de Empresas
                new Subject { Id = Guid.NewGuid(), Name = "Contabilidad General", Slug = "contabilidad-general", Code = "ADM101", CareerId = adminEmpresas.Id },
                new Subject { Id = Guid.NewGuid(), Name = "Gestión Empresarial", Slug = "gestion-empresarial", Code = "ADM202", CareerId = adminEmpresas.Id },

                // Marketing
                new Subject { Id = Guid.NewGuid(), Name = "Comportamiento del Consumidor", Slug = "comportamiento-del-consumidor", Code = "MKT101", CareerId = marketing.Id },
                new Subject { Id = Guid.NewGuid(), Name = "Publicidad Digital", Slug = "publicidad-digital", Code = "MKT202", CareerId = marketing.Id },

                // Derecho Empresarial
                new Subject { Id = Guid.NewGuid(), Name = "Derecho Civil I", Slug = "derecho-civil-i", Code = "DER101", CareerId = derechoEmp.Id },
                new Subject { Id = Guid.NewGuid(), Name = "Ética Profesional", Slug = "etica-profesional", Code = "DER202", CareerId = derechoEmp.Id }
            };
        }

        public static void Seed(UpsaMeDbContext db)
        {
            if (!db.Subjects.Any())
            {
                var subjects = GetSubjects(db);
                db.Subjects.AddRange(subjects);
                db.SaveChanges();
                Console.WriteLine("✅ Materias cargadas correctamente en la base de datos.");
            }
            else
            {
                Console.WriteLine("ℹ️ Las materias ya existen en la base de datos.");
            }
        }
    }
}
