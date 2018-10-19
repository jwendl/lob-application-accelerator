using System.Threading.Tasks;

namespace LobAccelerator.Library.Services.Interfaces
{
    public interface IStorageService
    {
        Task<bool> BlobExistsAsync(string blobName);
        Task<bool> UploadBlobAsync(string blobName, byte[] blobData);
        Task<byte[]> DownloadBlobAsync(string blobName);
        Task<bool> DeleteBlobAsync(string blobName);
    }
}
