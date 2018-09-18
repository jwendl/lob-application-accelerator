using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LobAccelerator.Library.Utils
{
    static class TokenCacheHelper
    {
        /// <summary>
        /// Get the user token cache
        /// </summary>
        /// <returns></returns>
        public static TokenCache GetUserCache()
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

        /// <summary>
        /// Path to the token cache
        /// </summary>
        public static readonly string CacheFilePath = ConfigurationManager.AppSettings["TokenCacheFilename"];

        private static readonly object FileLock = new object();

        // TODO prevent deadlock
        /// <summary>
        /// Risk deadlock
        /// </summary>
        /// <param name="args"></param>
        public static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                args.TokenCache.Deserialize(
                    StorageHelper.DownloadBlob(
                            ConfigurationManager.AppSettings["TokenCacheContainerName"],
                            ConfigurationManager.AppSettings["TokenCacheBlobName"],
                            CacheFilePath)
                        .GetAwaiter().GetResult()
                    ? ProtectedData.Unprotect(
                        File.ReadAllBytes(CacheFilePath),
                        null,
                        DataProtectionScope.CurrentUser)
                    : null);
            }
        }

        public static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.TokenCache.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changes in the persistent store
                    File.WriteAllBytes(CacheFilePath,
                                       ProtectedData.Protect(args.TokenCache.Serialize(),
                                                             null,
                                                             DataProtectionScope.CurrentUser)
                                      );
                    // once the write operationtakes place restore the HasStateChanged bit to filse
                    args.TokenCache.HasStateChanged = false;
                }
            }
        }
    }
}
