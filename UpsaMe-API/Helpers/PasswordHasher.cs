using System;
using System.Text;
using System.Security.Cryptography;
using Isopoh.Cryptography.Argon2;

namespace UpsaMe_API.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));

            // Elegimos Argon2id si existe en esta versión del paquete; si no, caemos a Argon2i
            var type = Enum.TryParse<Argon2Type>("HybridAddressing", out var t)
                ? t  // Argon2id en Isopoh
                : Argon2Type.DataIndependentAddressing; // Argon2i

            var config = new Argon2Config
            {
                Type = type,
                Version = Argon2Version.Nineteen,
                TimeCost = 4,
                MemoryCost = 1024 * 64, // 64 MB
                Lanes = 4,
                Threads = Math.Min(Environment.ProcessorCount, 4),
                Password = Encoding.UTF8.GetBytes(password),
                // RNG seguro en vez de GenerateSalt(...)
                Salt = RandomNumberGenerator.GetBytes(16),
                HashLength = 32
            };

            using var argon2 = new Argon2(config);
            var hash = argon2.Hash();
            return config.EncodeString(hash.Buffer);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            try { return Argon2.Verify(hash, password); }
            catch { return false; }
        }
    }
}