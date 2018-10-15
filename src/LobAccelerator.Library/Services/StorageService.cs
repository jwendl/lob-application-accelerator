using LobAccelerator.Library.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Services
{
    public class StorageService
        : IStorageService
    {
        private readonly CloudBlobContainer cloudBlobContainer;
        private readonly ILogger logger;

        public StorageService(string connectionString, Uri containerUri, ILogger logger)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var policy = new SharedAccessAccountPolicy()
            {
                // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request. 
                // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.List | SharedAccessAccountPermissions.Create | SharedAccessAccountPermissions.Delete,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Protocols = SharedAccessProtocol.HttpsOrHttp
            };

            var sasToken = cloudStorageAccount.GetSharedAccessSignature(policy);
            var storageCredentials = new StorageCredentials(sasToken);

            cloudBlobContainer = new CloudBlobContainer(containerUri, storageCredentials);
            this.logger = logger;
        }

        public async Task<bool> BlobExistsAsync(string blobName)
        {
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            return await cloudBlockBlob.ExistsAsync();
        }

        public async Task<bool> UploadBlobAsync(string blobName, byte[] blobData)
        {
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.UploadFromByteArrayAsync(blobData, 0, blobData.Length);
            return await Task.FromResult(true);
        }

        public async Task<byte[]> DownloadBlobAsync(string blobName)
        {
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.FetchAttributesAsync();

            var dataSize = cloudBlockBlob.Properties.Length;
            if (dataSize < 0)
            {
                return new byte[0];
            }

            var blobData = new byte[dataSize];
            var downloadedSize = await cloudBlockBlob.DownloadToByteArrayAsync(blobData, 0);

            if (cloudBlockBlob.Properties.Length != downloadedSize)
            {
                // TODO error check here?
            }

            return blobData;
        }

        public async Task<bool> DeleteBlobAsync(string blobName)
        {
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, null, null);
            return true;
        }
    }
}
