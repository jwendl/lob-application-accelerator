using Microsoft.SharePoint.Client;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Common;
using LobAccelerator.Library.Models.SharePoint;
using LobAccelerator.Library.Models.SharePoint.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;

namespace LobAccelerator.Library.Managers
{
    public class SharePointManager
        : ISharePointManager
    {
        private readonly IConfiguration configuration;
        private readonly ITokenManager tokenManager;

        public SharePointManager(IConfiguration configuration, ITokenManager tokenManager)
        {
            this.configuration = configuration;
            this.tokenManager = tokenManager;
        }

        public async Task<Result<SiteCollection>> CreateSiteCollectionAsync(SiteCollection siteCollection)
        {
            try
            {
                var result = new Result<SiteCollection>();
                var endpoint = $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com";
                var context = new ClientContext(endpoint);
                context.ExecutingWebRequest += ContextExecutingWebRequestAsync;
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
                context.ExecuteQuery();

                result.Value = siteCollection;
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private async void ContextExecutingWebRequestAsync(object sender, WebRequestEventArgs e)
        {
            var desiredScopes = new string[]
            {
                $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com/AllSites.FullControl"
            };
            var authResult = await tokenManager.GetOnBehalfOfAccessTokenAsync(desiredScopes);
            e.WebRequestExecutor.RequestHeaders.Add("Authorization", $"Bearer ${authResult.AccessToken}");
        }
    }
}
