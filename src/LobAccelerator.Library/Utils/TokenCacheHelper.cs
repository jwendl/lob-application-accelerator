using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Utils
{
    public interface ITokenCacheHelper
    {
        TokenCache FetchUserCache();
    }

    public class TokenCacheHelper
        : ITokenCacheHelper
    {
        private readonly IConfiguration configuration;

        public TokenCacheHelper(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Get the user token cache
        /// </summary>
        /// <returns></returns>
        public TokenCache FetchUserCache()
        {
            if (usertokenCache == null)
            {
                usertokenCache = new TokenCache();
                usertokenCache.SetBeforeAccess(BeforeAccessNotification);
                usertokenCache.SetAfterAccess(AfterAccessNotification);
            }
            return usertokenCache;
        }

        static TokenCache usertokenCache;

        private static readonly object FileLock = new object();

        // TODO prevent deadlock
        /// <summary>
        /// Risk deadlock
        /// </summary>
        /// <param name="args"></param>
        public void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                var existTask = StorageHelper.BlobExistsAsync(
                        configuration["TokenCacheHelper:StorageConnectionString"],
                        configuration["TokenCacheHelper:TokenCacheContainerName"],
                        configuration["TokenCacheHelper:TokenCacheBlobName"]);

                Task.WaitAll(existTask);

                if (existTask.Result)
                {
                    var fileTask = StorageHelper.DownloadBlobAsync(
                        configuration["TokenCacheHelper:StorageConnectionString"],
                        configuration["TokenCacheHelper:TokenCacheContainerName"],
                        configuration["TokenCacheHelper:TokenCacheBlobName"]);

                    Task.WaitAll(fileTask);

                    args.TokenCache.Deserialize(ProtectedData.Unprotect(fileTask.Result, null, DataProtectionScope.CurrentUser));
                }
                else
                {
                    args.TokenCache.Deserialize(null);
                }
            }
        }

        public void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.TokenCache.HasStateChanged)
            {
                lock (FileLock)
                {
                    var fileTask = StorageHelper.UploadBlobAsync(
                            configuration["TokenCacheHelper:StorageConnectionString"],
                            configuration["TokenCacheHelper:TokenCacheContainerName"],
                            configuration["TokenCacheHelper:TokenCacheBlobName"],
                            ProtectedData.Protect(args.TokenCache.Serialize(), null, DataProtectionScope.CurrentUser));

                    Task.WaitAll(fileTask);

                    // once the write operationtakes place restore the HasStateChanged bit to filse
                    args.TokenCache.HasStateChanged = false;
                }
            }
        }
    }
}
