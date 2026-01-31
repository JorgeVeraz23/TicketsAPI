using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration config)
        {
            var cs = config["BlobStorage:ConnectionString"]!;
            var container = config["BlobStorage:Container"]!;

            if (string.IsNullOrWhiteSpace(cs))
                throw new Exception("BlobStorage:ConnectionString está vacío o no se encontró en la configuración.");

            if (string.IsNullOrWhiteSpace(container))
                throw new Exception("BlobStorage:Container está vacío o no se encontró en la configuración.");

            _container = new BlobContainerClient(cs, container);
            _container.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> UploadAsync(Stream stream, string contentType, string blobPath, CancellationToken ct)
        {
            var blob = _container.GetBlobClient(blobPath);
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
            return blobPath;
        }

        public async Task<(Stream stream, string contentType)> DownloadAsync(string blobPath, CancellationToken ct)
        {
            var blob = _container.GetBlobClient(blobPath);
            var res = await blob.DownloadStreamingAsync(cancellationToken: ct);
            return (res.Value.Content, res.Value.Details.ContentType ?? "application/octet-stream");
        }

        public async Task DeleteIfExistsAsync(string blobPath, CancellationToken ct)
        {
            var blob = _container.GetBlobClient(blobPath);
            await blob.DeleteIfExistsAsync(cancellationToken: ct);
        }
    }
}
