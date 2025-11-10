using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UpsaMe_API.Config;
using UpsaMe_API.Data;
using UpsaMe_API.Data.Seed;
using UpsaMe_API.Helpers;
using UpsaMe_API.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================
// APPSETTINGS (bind a clases fuertemente tipadas)
// =============================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AzureSettings>(builder.Configuration.GetSection("AzureSettings"));

// =============================
// CORS (simple para dev; ajusta en prod)
// =============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// =============================
// DB (EF Core SQL Server)
// =============================
builder.Services.AddDbContext<UpsaMeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =============================
// Servicios (DI)
// =============================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<PostService>();

// Blob helper (singleton con connection string)
var blobConn = builder.Configuration.GetSection("AzureSettings")["BlobConnectionString"];
builder.Services.AddSingleton(new BlobStorageHelper(blobConn!));

// =============================
// JWT Auth
// =============================
var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // en prod: true detrás de HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// =============================
// Controllers + Swagger
// =============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UpsaMe API",
        Version = "v1",
        Description = "API para la plataforma UpsaMe (Ayudantes, Estudiantes, Comentarios)"
    });

    // Auth en Swagger (Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Usa: Bearer {tu_jwt}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// =============================
// Pipeline HTTP
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// =============================
// Seed inicial
// =============================
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    try
    {
        var db = sp.GetRequiredService<UpsaMeDbContext>();
        db.Database.Migrate(); // aplica migraciones pendientes

        // Tu inicializador es estático:
        DbInitializer.Seed(db);

        Console.WriteLine("✅ Datos iniciales cargados correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error ejecutando seed: {ex.Message}");
    }
}

app.Run();