using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Data;
using UpsaMe_API.DTOs.Auth;
using UpsaMe_API.Helpers;
using UpsaMe_API.Models;

namespace UpsaMe_API.Services
{
    public class AuthService
    {
        private readonly UpsaMeDbContext _db;
        private readonly TokenService _tokenService;

        public AuthService(UpsaMeDbContext db, TokenService tokenService)
        {
 Flavia
            _context = context;
            _jwt = config.GetSection("JwtSettings").Get<JwtSettings>()
                   ?? throw new InvalidOperationException("JwtSettings no configurado.");
        }

        // =======================
        // REGISTER
        // =======================
        public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto)
            _db = db;
            _tokenService = tokenService;
        }

        // ============================================================
        //  REGISTRO DE USUARIO
        // ============================================================
        public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
 main
        {
            ValidatePassword(dto.Password);

            var email = dto.Email.Trim().ToLower();

            // Verificar si el correo ya existe
            if (await _db.Users.AnyAsync(u => u.Email == email, ct))
                throw new InvalidOperationException("Ya existe un usuario con este correo institucional.");

            var user = new User
            {
                Email = email,
 Flavia
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                FirstName = dto.FirstName?.Trim() ?? string.Empty,
                LastName  = dto.LastName?.Trim()  ?? string.Empty,
                CareerId  = dto.CareerId,   // 👈 AQUÍ el GUID de la carrera
                Semester  = dto.Semester

                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Career = dto.Career?.Trim(),
                Semester = dto.Semester,
                PasswordHash = HashHelper.HashPassword(dto.Password) // importante
 main
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            return await GenerateTokensAsync(user);
        }

 Flavia
        // =======================
        // LOGIN
        // =======================
        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)

        // ============================================================
        //  LOGIN
        // ============================================================
        public async Task<TokenResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
 main
        {
            var email = dto.Email.Trim().ToLower();

 Flavia
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
 main

            if (user == null)
                throw new InvalidOperationException("Credenciales inválidas.");

            if (!HashHelper.VerifyPassword(dto.Password, user.PasswordHash))
                throw new InvalidOperationException("Credenciales inválidas.");

            return await GenerateTokensAsync(user);
        }

 Flavia
        // =======================
        // REFRESH TOKEN
        // =======================
        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new InvalidOperationException("Refresh token requerido.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
                throw new InvalidOperationException("Refresh token inválido.");

            if (!user.RefreshTokenExpiresAtUtc.HasValue ||
                user.RefreshTokenExpiresAtUtc.Value < DateTime.UtcNow)
                throw new InvalidOperationException("Refresh token expirado.");

            return await GenerateTokensAsync(user);
        }

        // =======================
        // GENERAR TOKENS
        // =======================
        private async Task<TokenResponseDto> GenerateTokensAsync(User user)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_jwt.Key);
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("fullName", $"{user.FirstName} {user.LastName}")
            };

            var accessToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                signingCredentials: creds
            );

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            var newRefreshToken = Guid.NewGuid().ToString("N");
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays);

            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessTokenString,
                RefreshToken = newRefreshToken,
                ExpiresAtUtc = accessToken.ValidTo
            };
        }

        // =======================
        // VALIDACIÓN DE PASSWORD
        // =======================

        // ============================================================
        //  REFRESH TOKEN (POR AHORA NO IMPLEMENTADO, PERO EVITA ERRORES)
        // ============================================================
        public Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException("Refresh token no implementado aún.");
        }

        // ============================================================
        //  VALIDACIÓN DE CONTRASEÑA
        // ============================================================
 main
        private static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new InvalidOperationException("La contraseña debe tener al menos 8 caracteres.");

            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);

            if (!(hasUpper && hasLower && hasDigit))
                throw new InvalidOperationException("La contraseña debe incluir mayúsculas, minúsculas y dígitos.");
        }

        // ============================================================
        //  GENERAR TOKENS JWT
        // ============================================================
        private TokenResponseDto GenerateTokens(User user)
        {
            var (access, refresh, expiresAt) = _tokenService.GenerateTokens(user);

            return new TokenResponseDto
            {
                AccessToken = access,
                RefreshToken = refresh,
                ExpiresAtUtc = expiresAt
            };
        }
    }
 Flavia
}

}

 main
