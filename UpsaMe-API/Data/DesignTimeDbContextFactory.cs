using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace UpsaMe_API.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UpsaMeDbContext>
    {
        public UpsaMeDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = config.GetConnectionString("DefaultConnection")
                       ?? "Server=(localdb)\\mssqllocaldb;Database=UpsaMeDev;Trusted_Connection=True;";

            var optionsBuilder = new DbContextOptionsBuilder<UpsaMeDbContext>();
            // Cambia UseSqlServer por UseNpgsql/UseMySql si usas otro proveedor
            optionsBuilder.UseSqlServer(conn);

            return new UpsaMeDbContext(optionsBuilder.Options);
        }
    }
}