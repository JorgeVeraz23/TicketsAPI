namespace TicketsAPI.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream stream, string contentType, string blobPath, CancellationToken ct);
        Task<(Stream stream, string contentType)> DownloadAsync(string blobPath, CancellationToken ct);
        Task DeleteIfExistsAsync(string blobPath, CancellationToken ct);
        // ✅ NUEVO
        string GetReadSasUrl(string blobPath, int expiresMinutes = 10);
    }
}
