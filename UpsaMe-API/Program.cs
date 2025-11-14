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
// APPSETTINGS (tipados)
// =============================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AzureSettings>(builder.Configuration.GetSection("AzureSettings"));

// =============================
// CORS
// =============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// =============================
// DB
// =============================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró 'ConnectionStrings:DefaultConnection'.");

builder.Services.AddDbContext<UpsaMeDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

// =============================
// Servicios
// =============================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<PostService>();

// Blob helper
var blobConn = builder.Configuration.GetSection("AzureSettings")["BlobConnectionString"];
if (string.IsNullOrWhiteSpace(blobConn))
    throw new InvalidOperationException("AzureSettings:BlobConnectionString no configurado.");

builder.Services.AddSingleton(new BlobStorageHelper(blobConn));

// =============================
// JWT
// =============================
var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
          ?? throw new InvalidOperationException("JwtSettings no configurado.");

if (string.IsNullOrWhiteSpace(jwt.Key))
    throw new InvalidOperationException("JwtSettings:Key no puede estar vacío.");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // dev
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
        Description = "API para la plataforma UpsaMe"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Pega aquí SOLO el JWT (sin 'Bearer ').",
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
// Pipeline
// =============================

// 👇 Para ver errores claros en dev
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// CORS
app.UseCors("AllowAll");

// 👇 TEMPORALMENTE COMENTAMOS HTTPS PARA QUITAR DRAMA
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 👇 Swagger accesible directo en raíz: http://localhost:xxxx/
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UpsaMe API v1");
    c.RoutePrefix = string.Empty; // Swagger en "/"
});

app.MapControllers();

// =============================
// Seed (igual que antes, pero solo loguea)
// =============================
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    try
    {
        var db = sp.GetRequiredService<UpsaMeDbContext>();
        db.Database.Migrate();
        DbInitializer.Seed(db);
        Console.WriteLine("✅ Datos iniciales cargados correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error ejecutando seed: {ex.Message}");
    }
}

app.Run();
