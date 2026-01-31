namespace TicketsAPI.Interfaces
{
    public interface IFileStorage
    {
        Task<string> UploadAsync(Stream stream, string contentType, string fileName);
        Task<(Stream stream, string contentType)> DownloadAsync(string blobName);
    }

}
