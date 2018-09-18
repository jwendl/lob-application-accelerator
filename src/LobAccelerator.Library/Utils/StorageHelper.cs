using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

// https://github.com/Azure-Samples/storage-blob-dotnet-getting-started

namespace LobAccelerator.Library.Utils
{
    public static class StorageHelper
    {
        /// <summary>
        /// Upload bytes as a blob to the passed in container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <param name="blobData"></param>
        /// <returns></returns>
        public static async Task<bool> UploadBlob(string containerName, string blobName, byte[] blobData)
        {
            try
            {
                var container = await GetContainer(containerName);

                // Upload a BlockBlob to the newly created container
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                await blockBlob.UploadFromByteArrayAsync(blobData, 0, blobData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Downloads entire blob locally with passed in filename. Will overwrite file if it exists.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public static async Task<bool> DownloadBlob(string containerName, string blobName, string writeFile)
        {
            try
            {
                var container = await GetContainer(containerName);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                await blockBlob.DownloadToFileAsync(writeFile, FileMode.Create);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes the blob of this name in the given container, and all its snapshots.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteBlob(string containerName, string blobName)
        {
            try
            {
                var container = await GetContainer(containerName);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                await blockBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return false;
            }

            return true;
        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            const string errorMessage = "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.";

            try
            {
                storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            }
            catch (FormatException)
            {
                Console.WriteLine(errorMessage);
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(errorMessage);
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        /// <summary>
        /// Creates the container if it doesn't exist. Returns the container object
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private static async Task<CloudBlobContainer> GetContainer(string containerName)
        {
            // Get account using sas token
            var sasToken = GetAccountSASToken();
            StorageCredentials accountSAS = new StorageCredentials(sasToken);

            // create the storage container if necessary
            Uri containerUri = GetContainerUri(containerName);
            CloudBlobContainer container = new CloudBlobContainer(containerUri, accountSAS);
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("If you are running with the default configuration, please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                return null;
            }

            return container;
        }

        private static string GetAccountSASToken()
        {
            // Retrieve storage account information from connection string
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();
            
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

        private static Uri GetContainerUri(string containerName)
        {
            // Retrieve storage account information from connection string
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();

            return storageAccount.CreateCloudBlobClient().GetContainerReference(containerName).Uri;
        }
    }
}
