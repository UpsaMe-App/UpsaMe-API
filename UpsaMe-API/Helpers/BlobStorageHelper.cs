using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace UpsaMe_API.Helpers
{
    public class BlobStorageHelper
    {
        private const string ContainerName = "profilephotos";
        private readonly BlobContainerClient _containerClient;

        public BlobStorageHelper(string connectionString)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        }

        public async Task<string> UploadProfilePhotoAsync(Guid userId, Stream fileStream, string? contentType)
        {
            try
            {
                // Crea el contenedor si no existe (acceso público solo a blobs)
                await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                // Asegura un content-type válido
                contentType = string.IsNullOrWhiteSpace(contentType) ? "image/jpeg" : contentType;

                // Genera nombre único para evitar cache y colisiones
                var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}.jpg";
                var blobClient = _containerClient.GetBlobClient(fileName);

                // Sube el archivo (sobrescribe si existía) 
                // Nota: con este overload NO se pueden pasar headers todavía
                fileStream.Position = 0; // por si el stream ya fue leído
                await blobClient.UploadAsync(fileStream, overwrite: true);

                // Setea Content-Type después de subir
                await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
                {
                    ContentType = contentType
                });

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al subir la foto de perfil: {ex.Message}", ex);
            }
        }
    }
}