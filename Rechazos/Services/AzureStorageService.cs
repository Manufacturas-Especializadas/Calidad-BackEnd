using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;

namespace Rechazos.Services
{
    public class AzureStorageService
    {
        private readonly string _connectionString;
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorageConnection")!;
            _blobServiceClient = new BlobServiceClient(_connectionString);
        }

        public async Task<string> StoragePhotos(string container, IFormFile photo)
        {
            try
            {
                if(photo == null || photo.Length == 0)
                {
                    throw new ApplicationException("El archivo adjunto no puede estar vacio");
                }

                var extension = Path.GetExtension(photo.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

                if(!allowedExtensions.Contains(extension))
                {
                    throw new ApplicationException("Solo se permiten archivos de imagen (.jpg, .jpeg, .png, .gif, .bmp, .webp)");
                }

                var client = _blobServiceClient.GetBlobContainerClient(container);
                await client.CreateIfNotExistsAsync();

                var fileName = photo.FileName;
                var blob = client.GetBlobClient(fileName);

                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    _ => "application/octet-stream"
                };

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                await blob.UploadAsync(photo.OpenReadStream(), blobHttpHeaders);

                return blob.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al guardar el archivo", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string container, string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return false;
            }

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);

                var blobName = new Uri(fileUrl).Segments.LastOrDefault();

                if (string.IsNullOrEmpty(blobName))
                {
                    return false;
                }

                var blobClient = containerClient.GetBlobClient(blobName);

                var response = await blobClient.DeleteIfExistsAsync();

                return response.Value;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error al eliminar el blob: {ex.Message}");

                return false;
            }
        }
    }
}