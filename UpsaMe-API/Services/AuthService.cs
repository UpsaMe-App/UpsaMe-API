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
            _db = db;
            _tokenService = tokenService;
        }

        // ============================================================
        //  REGISTRO DE USUARIO
        // ============================================================
        public async Task<TokenResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            ValidatePassword(dto.Password);

            var email = dto.Email.Trim().ToLower();

            // Verificar si el correo ya existe
            if (await _db.Users.AnyAsync(u => u.Email == email, ct))
                throw new InvalidOperationException("Ya existe un usuario con este correo institucional.");

            var user = new User
            {
                Email = email,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Career = dto.Career?.Trim(),
                Semester = dto.Semester,
                PasswordHash = HashHelper.HashPassword(dto.Password) // importante
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            return GenerateTokens(user);
        }

        // ============================================================
        //  LOGIN
        // ============================================================
        public async Task<TokenResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var email = dto.Email.Trim().ToLower();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user == null)
                throw new InvalidOperationException("Credenciales inválidas.");

            if (!HashHelper.VerifyPassword(dto.Password, user.PasswordHash))
                throw new InvalidOperationException("Credenciales inválidas.");

            return GenerateTokens(user);
        }

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
}

