using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UpsaMe_API.Data;
using UpsaMe_API.Data.Seed;
using UpsaMe_API.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================
// CONFIGURACIÓN GENERAL
// =============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// =============================
// BASE DE DATOS
// =============================
builder.Services.AddDbContext<UpsaMeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =============================
// SERVICIOS
// =============================
// Como no hay interfaces, registrá las clases directamente:
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<PostService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// =============================
// SWAGGER / OPENAPI
// =============================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UpsaMe API",
        Version = "v1",
        Description = "API para la plataforma UpsaMe (Ayudantes, Estudiantes, Comentarios)"
    });
});

var app = builder.Build();

// =============================
// PIPELINE HTTP
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();

// =============================
// SEED INICIAL
// =============================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<UpsaMeDbContext>();
        db.Database.Migrate();

        // DbInitializer es estático y su método real es Seed(db)
        DbInitializer.Seed(db);

        Console.WriteLine("✅ Datos iniciales cargados correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error ejecutando seed: {ex.Message}");
    }
}

app.Run();
