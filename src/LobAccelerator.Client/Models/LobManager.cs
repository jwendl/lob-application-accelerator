using LobAccelerator.Client.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Client.Models
{
    public class LobManager
    {
        private readonly string _endpoint;
        private readonly string _resource;
        private readonly string _clientId;
        private readonly IEnumerable<string> _files;

        public LobManager(Options options)
        {
            var configuration = new ConfigurationManager(options.ConfigurationFile);

            _endpoint = configuration["LobEngine:Endpoint"];
            _resource = configuration["AzureAd:Resource"];
            _clientId = configuration["AzureAd:ClientId"];
            _files = options.DefinitionsFiles;
        }

        public async Task ProvisionResourcesAsync()
        {
            var accessToken = await ConsoleExtensions.GetTokenByCode(_resource, _clientId);
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Authorization", $"bearer {accessToken}");
                _files.ToList().ForEach(async f => await RequestProvisioning(f, client));
            }
        }

        private async Task RequestProvisioning(string file, HttpClient client)
        {
            var content = await file.GetFileContentAsync();
            var body = new StringContent(content, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(_endpoint, body);
            response.EnsureSuccessStatusCode();
        }
    }
}
