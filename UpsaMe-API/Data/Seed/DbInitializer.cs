namespace UpsaMe_API.Data.Seed
{
    public static class DbInitializer
    {
        public static void Seed(UpsaMeDbContext db)
        {
            Console.WriteLine("🚀 Iniciando carga de datos iniciales UPSA...");

            try
            {
                FacultySeed.Upsert(db);
                CareerSeed.Upsert(db);
                SubjectSeed.Seed(db); // ya debe ser idempotente

                Console.WriteLine("✅ Datos iniciales UPSA cargados o actualizados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error durante el seed: {ex.Message}");
                throw;
            }
        }
    }
}