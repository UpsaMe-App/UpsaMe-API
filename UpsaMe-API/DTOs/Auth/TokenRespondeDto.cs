namespace UpsaMe_API.DTOs.Auth
{
    public class TokenResponseDto
    {
        /// <summary>
        /// Token JWT de acceso (válido por un período corto).
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Token de actualización (permite solicitar un nuevo AccessToken sin volver a iniciar sesión).
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora (UTC) en que el AccessToken expira.
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}