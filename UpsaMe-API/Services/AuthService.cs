using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly JwtSettings _jwtSettings;
        private const string UpsaEmailRegex = @"^a(2005|2006|2007|2008|2009|201[0-9]|202[0-5])\d{6}@estudiantes\.upsa\.edu\.bo$";

        public AuthService(UpsaMeDbContext context, IConfiguration config)
        {
            _context = context;
            _jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>()!;
        }

        public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Email, UpsaEmailRegex))
                throw new Exception("Email institucional no válido.");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("El correo ya está registrado.");

            var hash = PasswordHasher.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email.ToLower(),
                PasswordHash = hash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Career = dto.Career,
                Semester = dto.Semester
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateTokens(user);
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                throw new Exception("Credenciales incorrectas.");

            return GenerateTokens(user);
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // Aquí más adelante se implementa persistencia de refresh tokens
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        private TokenResponseDto GenerateTokens(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new("fullName", $"{user.FirstName} {user.LastName}")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenResponseDto
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString(),
                ExpiresAtUtc = tokenDescriptor.Expires!.Value
            };
        }
    }
}
