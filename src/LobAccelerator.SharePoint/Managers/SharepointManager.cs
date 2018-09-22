using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.SharePoint.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class SharePointManager
        : ISharePointManager
    {
        private readonly IConfiguration configuration;
        private readonly ITokenManager tokenManager;
        private readonly string accessToken;

        public SharePointManager(IConfiguration configuration, ITokenManager tokenManager, string accessToken)
        {
            this.configuration = configuration;
            this.tokenManager = tokenManager;
            this.accessToken = accessToken;
        }

        public async Task<Result<SiteCollection>> CreateSiteCollectionAsync(SiteCollection siteCollection)
        {
            try
            {
                var result = new Result<SiteCollection>();
                var endpoint = $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com";
                var context = new ClientContext(endpoint);
                context.ExecutingWebRequest += ContextExecutingWebRequest;
                var tenant = new Tenant(context);

                var properties = new SiteCreationProperties()
                {
                    Url = siteCollection.Url,
                    Owner = siteCollection.Owner,
                    Title = siteCollection.Title,
                    Template = siteCollection.Template,
                    StorageMaximumLevel = siteCollection.StorageMaximumLevel,
                    UserCodeMaximumLevel = siteCollection.UserCodeMaximumLevel
                };

                var op = tenant.CreateSite(properties);
                context.Load(tenant);
                context.Load(op, i => i.IsComplete);
                // TODO: Fails with 400 Bad Request
                context.ExecuteQuery();

                result.Value = siteCollection;
                return await Task.FromResult(result);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private void ContextExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            var desiredScopes = new string[]
            {
                $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com/AllSites.FullControl"
            };
            var authResultTask = tokenManager.GetOnBehalfOfAccessTokenAsync(accessToken, desiredScopes);

            Task.WaitAll(authResultTask);

            e.WebRequestExecutor.RequestHeaders.Add("Authorization", $"Bearer ${authResultTask.Result.AccessToken}");
        }
    }
}
