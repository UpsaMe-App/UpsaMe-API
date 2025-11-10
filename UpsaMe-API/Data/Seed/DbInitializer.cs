namespace UpsaMe_API.Data.Seed
 {
    public static class DbInitializer
    {
        public static void Seed(UpsaMeDbContext db)
        {
            db.Database.EnsureCreated();

            if (!db.Faculties.Any())
            {
                var faculties = FacultySeed.GetFaculties();
                db.Faculties.AddRange(faculties);
                db.SaveChanges();

                var careers = CareerSeed.GetCareers(faculties);
                db.Careers.AddRange(careers);
                db.SaveChanges();

                // Corregido: SubjectSeed ahora usa el contexto directamente
                SubjectSeed.Seed(db);

                Console.WriteLine("✅ Datos iniciales UPSA cargados correctamente.");
            }
            else
            {
                Console.WriteLine("ℹ️ Los datos iniciales ya existen.");
            }
        }
    }
}
