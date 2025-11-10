using UpsaMe_API.Models;

namespace UpsaMe_API.Data.Seed
{
    public static class CareerSeed
    {
        public static List<Career> GetCareers(List<Faculty> faculties)
        {
            // Busca las facultades por slug y falla con mensaje claro si no existen
            var engineering  = faculties.FirstOrDefault(f => f.Slug == "ingenieria")
                ?? throw new InvalidOperationException("Falta facultad con slug 'ingenieria' en FacultySeed.");
            var business     = faculties.FirstOrDefault(f => f.Slug == "empresariales")
                ?? throw new InvalidOperationException("Falta facultad con slug 'empresariales' en FacultySeed.");
            var law          = faculties.FirstOrDefault(f => f.Slug == "juridicas")
                ?? throw new InvalidOperationException("Falta facultad con slug 'juridicas' en FacultySeed.");
            var arquitectura = faculties.FirstOrDefault(f => f.Slug == "arquitectura")
                ?? throw new InvalidOperationException("Falta facultad con slug 'arquitectura' en FacultySeed.");
            var humanidades  = faculties.FirstOrDefault(f => f.Slug == "humanidades")
                ?? throw new InvalidOperationException("Falta facultad con slug 'humanidades' en FacultySeed.");

            return new List<Career>
            {
                // ================== INGENIERÍA ==================
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería de Sistemas",                   Slug = "ing-sistemas",       FacultyId = engineering.Id },
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería Industrial y de Sistemas",     Slug = "ing-industrial",     FacultyId = engineering.Id },
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería Robótica y Mecatrónica",       Slug = "ing-mecatronica",    FacultyId = engineering.Id },
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería Informática y Administrativa", Slug = "ing-informatica",    FacultyId = engineering.Id },
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería Civil",                         Slug = "ing-civil",          FacultyId = engineering.Id },

                // ============== CIENCIAS EMPRESARIALES ==========
                new Career { Id = Guid.NewGuid(), Name = "Administración de Empresas",              Slug = "adm-empresas",       FacultyId = business.Id },
                new Career { Id = Guid.NewGuid(), Name = "Auditoría y Finanzas",                    Slug = "auditoria-finanzas", FacultyId = business.Id },
                new Career { Id = Guid.NewGuid(), Name = "Comercio Internacional",                  Slug = "comercio-int",       FacultyId = business.Id },
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería Económica",                    Slug = "ing-economica",      FacultyId = business.Id },
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería Financiera",                   Slug = "ing-financiera",     FacultyId = business.Id },
                new Career { Id = Guid.NewGuid(), Name = "Marketing y Publicidad",                  Slug = "marketing",          FacultyId = business.Id },
                new Career { Id = Guid.NewGuid(), Name = "Ingeniería Comercial",                    Slug = "ing-comercial",      FacultyId = business.Id },

                // ====== ARQUITECTURA, DISEÑO Y URBANISMO (ADU) ===
                new Career { Id = Guid.NewGuid(), Name = "Arquitectura",                            Slug = "arquitectura",       FacultyId = arquitectura.Id },
                new Career { Id = Guid.NewGuid(), Name = "Diseño Industrial",                       Slug = "diseno-industrial",  FacultyId = arquitectura.Id },

                // === HUMANIDADES, COMUNICACIÓN Y ARTES ===========
                // OJO: todas estas pertenecen a humanidades.Id
                new Career { Id = Guid.NewGuid(), Name = "Comunicación Estratégica y Corporativa",  Slug = "comunicacion",       FacultyId = humanidades.Id },
                new Career { Id = Guid.NewGuid(), Name = "Diseño y Gestión de la Moda",             Slug = "diseno-moda",        FacultyId = humanidades.Id },
                new Career { Id = Guid.NewGuid(), Name = "Diseño Gráfico",                          Slug = "diseno-grafico",     FacultyId = humanidades.Id },
                new Career { Id = Guid.NewGuid(), Name = "Psicología",                              Slug = "psicologia",         FacultyId = humanidades.Id },

                // ================== JURÍDICAS ====================
                new Career { Id = Guid.NewGuid(), Name = "Derecho",                                 Slug = "derecho",            FacultyId = law.Id },
            };
        }

        /// <summary>
        /// Inserta solo las carreras faltantes (por Slug). Idempotente.
        /// </summary>
        public static void Upsert(UpsaMeDbContext db)
        {
            var faculties = db.Faculties.ToList();

            // Verificación mínima de facultades requeridas
            var required = new[] { "ingenieria", "empresariales", "juridicas", "arquitectura", "humanidades" };
            var missing  = required.Where(slug => !faculties.Any(f => f.Slug == slug)).ToList();
            if (missing.Any())
                throw new InvalidOperationException("Faltan facultades requeridas para generar carreras: " + string.Join(", ", missing));

            var desired = GetCareers(faculties);
            var existingSlugs = db.Careers.Select(c => c.Slug).ToHashSet();

            var toAdd = desired.Where(c => !existingSlugs.Contains(c.Slug)).ToList();
            if (toAdd.Count > 0)
            {
                db.Careers.AddRange(toAdd);
                db.SaveChanges();
                Console.WriteLine($"✅ Se agregaron {toAdd.Count} carreras nuevas.");
            }
            else
            {
                Console.WriteLine("ℹ️ No hay carreras nuevas para agregar.");
            }
        }

        // Compatibilidad con tu DbInitializer actual
        public static void Seed(UpsaMeDbContext db) => Upsert(db);
    }
}

