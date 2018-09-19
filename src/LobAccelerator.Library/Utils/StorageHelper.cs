using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

// https://github.com/Azure-Samples/storage-blob-dotnet-getting-started

namespace LobAccelerator.Library.Utils
{
    public static class StorageHelper
    {
        /// <summary>
        /// Check if the blob exists
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public static async Task<bool> BlobExistsAsync(string connectionString, string containerName, string blobName)
        {
            try
            {
                var container = await GetContainerAsync(connectionString, containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                return await blockBlob.ExistsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
                return false;
            }
        }

        /// <summary>
        /// Upload bytes as a blob to the passed in container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <param name="blobData"></param>
        /// <returns></returns>
        public static async Task<bool> UploadBlobAsync(string connectionString, string containerName, string blobName, byte[] blobData)
        {
            try
            {
                var container = await GetContainerAsync(connectionString, containerName);

                // Upload a BlockBlob to the newly created container
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                await blockBlob.UploadFromByteArrayAsync(blobData, 0, blobData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns blob as bytes. Will return empty byte array if blob not found.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadBlobAsync(string connectionString, string containerName, string blobName)
        {
            try
            {
                var container = await GetContainerAsync(connectionString, containerName);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                await blockBlob.FetchAttributesAsync();

                var dataSize = blockBlob.Properties.Length;
                if (dataSize < 0)
                {
                    return new byte[0];
                }

                var blobData = new byte[dataSize];
                var downloadedSize = await blockBlob.DownloadToByteArrayAsync(blobData, 0);

                if (blockBlob.Properties.Length != downloadedSize)
                {
                    // TODO error check here?
                }

                return blobData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new byte[0];
            }
        }

        /// <summary>
        /// Deletes the blob of this name in the given container, and all its snapshots.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteBlobAsync(string connectionString, string containerName, string blobName)
        {
            try
            {
                var container = await GetContainerAsync(connectionString, containerName);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                await blockBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
                return false;
            }

            return true;
        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string connectionString)
        {
            CloudStorageAccount storageAccount;
            const string errorMessage = "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.";

            try
            {
                storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine(errorMessage);
                
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(errorMessage);
                
                throw;
            }

            return storageAccount;
        }

        /// <summary>
        /// Creates the container if it doesn't exist. Returns the container object
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private static async Task<CloudBlobContainer> GetContainerAsync(string connectionString, string containerName)
        {
            // Get account using sas token
            var sasToken = GetAccountSASToken(connectionString);
            StorageCredentials accountSAS = new StorageCredentials(sasToken);

            // create the storage container if necessary
            Uri containerUri = GetContainerUri(connectionString, containerName);
            CloudBlobContainer container = new CloudBlobContainer(containerUri, accountSAS);
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("If you are running with the default configuration, please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                
                return null;
            }

            return container;
        }

        private static string GetAccountSASToken(string connectionString)
        {
            // Retrieve storage account information from connection string
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(connectionString);

            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request. 
                // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.List | SharedAccessAccountPermissions.Create | SharedAccessAccountPermissions.Delete,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Protocols = SharedAccessProtocol.HttpsOrHttp
            };

            // Create new storage credentials using the SAS token.
            string sasToken = storageAccount.GetSharedAccessSignature(policy);

            // Return the SASToken
            return sasToken;
        }

        private static Uri GetContainerUri(string connectionString, string containerName)
        {
            // Retrieve storage account information from connection string
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(connectionString);

            return storageAccount.CreateCloudBlobClient().GetContainerReference(containerName).Uri;
        }
    }
}
