using Isopoh.Cryptography.Argon2;

namespace UpsaMe_API.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return Argon2.Hash(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return Argon2.Verify(hash, password);
        }
    }
}