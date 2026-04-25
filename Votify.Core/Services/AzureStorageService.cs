using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class AzureStorageService : IAzureStorage
    {
        private readonly BlobContainerClient _containerClient;

        public AzureStorageService(IConfiguration config)
        {
            var connectionString = config["AzureStorage:ConnectionString"];
            var containerName = config["AzureStorage:Container"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> SubirArchivoAsync(Stream contenidoArchivo, string nombreArchivo)
        {
            await _containerClient.CreateIfNotExistsAsync();

            // nombre único para evitar conflictos
            var nombreSeguro = $"{Guid.NewGuid()}_{nombreArchivo}";

            var blobClient = _containerClient.GetBlobClient(nombreSeguro);

            await blobClient.UploadAsync(contenidoArchivo, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task EliminarArchivoAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            var uri = new Uri(url);

            // obtiene solo el nombre del archivo
            var blobName = Path.GetFileName(uri.LocalPath);

            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}