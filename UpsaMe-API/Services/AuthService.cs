using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UpsaMe_API.Config;
using UpsaMe_API.Data;
using UpsaMe_API.DTOs.Auth;
using UpsaMe_API.Helpers;
using UpsaMe_API.Models;

namespace UpsaMe_API.Services
{
    public class AuthService
    {
        private readonly UpsaMeDbContext _context;
        private readonly JwtSettings _jwt;

        // a(2005–2025) + 6 dígitos + dominio
        private static readonly Regex UpsaEmailRegex =
            new(@"^a(2005|2006|2007|2008|2009|201[0-9]|202[0-5])\d{6}@estudiantes\.upsa\.edu\.bo$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public AuthService(UpsaMeDbContext context, IConfiguration config)
        {
            _context = context;
            _jwt = config.GetSection("JwtSettings").Get<JwtSettings>() 
                   ?? throw new InvalidOperationException("JwtSettings no configurado.");
        }

        public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto)
        {
            var email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (!UpsaEmailRegex.IsMatch(email))
                throw new InvalidOperationException("Email institucional no válido.");

            ValidatePassword(dto.Password);

            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            if (exists)
                throw new InvalidOperationException("El correo ya está registrado.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                FirstName = dto.FirstName?.Trim() ?? string.Empty,
                LastName = dto.LastName?.Trim() ?? string.Empty,
                Career = dto.Career,
                Semester = dto.Semester
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateTokens(user);
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            var email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();

            var user = await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new InvalidOperationException("Credenciales incorrectas.");

            if (string.IsNullOrEmpty(user.PasswordHash) ||
                !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                throw new InvalidOperationException("Credenciales incorrectas.");

            return GenerateTokens(user);
        }

        public Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // TODO: persistir y validar refresh tokens (tabla + rotación)
            throw new NotImplementedException("Refresh token no implementado todavía.");
        }

        private TokenResponseDto GenerateTokens(User user)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_jwt.Key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("fullName", $"{user.FirstName} {user.LastName}")
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                signingCredentials: creds
            );

            return new TokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString(), // placeholder hasta implementar
                ExpiresAtUtc = token.ValidTo
            };
        }

        private static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new InvalidOperationException("La contraseña debe tener al menos 8 caracteres.");

            // opcionalmente fuerza complejidad:
            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            if (!(hasUpper && hasLower && hasDigit))
                throw new InvalidOperationException("La contraseña debe incluir mayúsculas, minúsculas y dígitos.");
        }
    }
}
