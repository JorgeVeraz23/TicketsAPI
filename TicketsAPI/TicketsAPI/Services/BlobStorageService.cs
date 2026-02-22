using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _container;
        private readonly StorageSharedKeyCredential _sharedKey;
        private readonly string _containerName;
        private readonly Uri _containerUri;

        public BlobStorageService(IConfiguration config)
        {
            var cs = config["BlobStorage:ConnectionString"]!;
            var container = config["BlobStorage:Container"]!;

            if (string.IsNullOrWhiteSpace(cs))
                throw new Exception("BlobStorage:ConnectionString está vacío o no se encontró.");

            if (string.IsNullOrWhiteSpace(container))
                throw new Exception("BlobStorage:Container está vacío o no se encontró.");

            _container = new BlobContainerClient(cs, container);
            _container.CreateIfNotExists(PublicAccessType.None);

            _containerName = container;
            _containerUri = _container.Uri;

            // ✅ sacar AccountName + AccountKey del connection string
            var parts = cs.Split(';', StringSplitOptions.RemoveEmptyEntries)
                          .Select(x => x.Split('=', 2))
                          .Where(x => x.Length == 2)
                          .ToDictionary(x => x[0], x => x[1]);

            if (!parts.TryGetValue("AccountName", out var accountName) ||
                !parts.TryGetValue("AccountKey", out var accountKey))
            {
                throw new Exception("El ConnectionString no contiene AccountName/AccountKey (necesario para SAS con SharedKey).");
            }

            _sharedKey = new StorageSharedKeyCredential(accountName, accountKey);
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

        // ✅ NUEVO: SAS URL de lectura
        public string GetReadSasUrl(string blobPath, int expiresMinutes = 10)
        {
            if (string.IsNullOrWhiteSpace(blobPath))
                throw new ArgumentException("blobPath vacío.");

            var blobUri = new Uri($"{_containerUri.AbsoluteUri.TrimEnd('/')}/{blobPath.TrimStart('/')}");
            var builder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobPath,
                Resource = "b", // blob
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes)
            };

            builder.SetPermissions(BlobSasPermissions.Read);

            var sas = builder.ToSasQueryParameters(_sharedKey).ToString();
            return $"{blobUri}?{sas}";
        }
    }
}
