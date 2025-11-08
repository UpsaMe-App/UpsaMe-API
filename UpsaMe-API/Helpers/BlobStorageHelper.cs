using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace UpsaMe_API.Helpers
{
    public class BlobStorageHelper
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "profilephotos";

        public BlobStorageHelper(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadProfilePhotoAsync(Guid userId, Stream fileStream, string contentType)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var blobClient = container.GetBlobClient($"{userId}.jpg");

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}