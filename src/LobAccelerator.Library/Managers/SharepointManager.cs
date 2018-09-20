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
        private readonly HttpClient httpClient;

        public SharePointManager(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<Result<SiteCollection>> CreateSiteCollectionAsync(SiteCollection siteCollection)
        {
            try
            {
                var result = new Result<SiteCollection>();
                var endpoint = $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com";
                var context = new ClientContext(endpoint);
                context.ExecutingWebRequest += Context_ExecutingWebRequest;
                var tenant = new Tenant(context);
                var webUrl = $"https://{configuration["SharePointTenantPrefix"]}.sharepoint.com/sites/rztest123";
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
                var x = ex;
                throw ex;
            }
        }

        private static void Context_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            e.WebRequestExecutor.RequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Imk2bEdrM0ZaenhSY1ViMkMzbkVRN3N5SEpsWSIsImtpZCI6Imk2bEdrM0ZaenhSY1ViMkMzbkVRN3N5SEpsWSJ9.eyJhdWQiOiJodHRwczovL3J6bmEtYWRtaW4uc2hhcmVwb2ludC5jb20iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83Nzk4YTBlZS0xODc1LTQ2MjQtOGRlOC03N2QyMWY4MTQwZDAvIiwiaWF0IjoxNTM3NDc0MjY2LCJuYmYiOjE1Mzc0NzQyNjYsImV4cCI6MTUzNzQ3ODE2NiwiYWNyIjoiMSIsImFpbyI6IjQyQmdZT2pnOTR2eGY3UXA2bGlDM0JLbHlqREhOOXd2Q21SeUVqSXFDLytIckcxYXNSWUEiLCJhbXIiOlsicHdkIl0sImFwcF9kaXNwbGF5bmFtZSI6IlNpdGVDb2xsZWN0aW9uQ3JlYXRlIiwiYXBwaWQiOiJmNzIyNDcxZi03NDY1LTQwZWEtOTE5YS04NjVlYzgwZmZmZDgiLCJhcHBpZGFjciI6IjEiLCJmYW1pbHlfbmFtZSI6ImRpWmVyZWdhIiwiZ2l2ZW5fbmFtZSI6IlJpY2hhcmQiLCJpcGFkZHIiOiI2OC4yMDMuMjcuMjMxIiwibmFtZSI6IlJpY2hhcmQgZGlaZXJlZ2EiLCJvaWQiOiJjY2FmNDhhMi0wZmUzLTRlMmQtODkxMy04MjljMGYxZGNmYWMiLCJwdWlkIjoiMTAwM0JGRkQ5Q0NDNDYyNCIsInNjcCI6IkFsbFNpdGVzLkZ1bGxDb250cm9sIiwic2lnbmluX3N0YXRlIjpbImttc2kiXSwic3ViIjoielBBamdrRk9EenNVaEZ3QUtBcDBkNUZZd24tMHlCcnZoTzFsM3lDamxVYyIsInRpZCI6Ijc3OThhMGVlLTE4NzUtNDYyNC04ZGU4LTc3ZDIxZjgxNDBkMCIsInVuaXF1ZV9uYW1lIjoicmljaEByaWNoZGl6ei5jb20iLCJ1cG4iOiJyaWNoQHJpY2hkaXp6LmNvbSIsInV0aSI6Im14Tk5Wd2pYUkVLSjRyUzRPZUVGQUEiLCJ2ZXIiOiIxLjAifQ.WAZvD1hxl5v3Di1CYA5TtTkk-_bxHOX6xkchdYpo6Gxgq3Ct_J-YvJgzKeeT66UK8kdTiswXYGSa8KWoZz2VrbsLd76xXa3CrECTokuam53pLgNsatvxEG1YjHQ0rgdxjBGBkz4N9txdwUFkqE9gp_mm9rON0PXUhckPMZInaZzuYP4nLCZyUIliMKOaZRVWJkWhn049zZ0PL8pQsDCWN_9s9GG7mHZE2LU--PEr0PIgcMWsN0FsuuDbHbJc7hOx2IISeWLC-t-W9DzgL9TSdUwkjWOoUuyx8GrjoXZlWfotcEvd5I368Apb2535xj-ZeKU1mcOCyaVKlzu6toNKcg");
        }
    }
}
