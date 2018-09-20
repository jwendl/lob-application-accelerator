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
        private readonly Configuration _config;
        private readonly IEnumerable<string> _files;

        public LobManager(Options options)
        {
            _config = GetConfiguration(options.ConfigurationFile);
            _files = options.DefinitionsFiles;
        }

        private Configuration GetConfiguration(string configFile)
        {
            var configuration = new ConfigurationManager(configFile);

            return new Configuration
            {
                Endpoint = configuration["LobEngine:Endpoint"],
                AzureAd = new AzureAd
                {
                    Resource = configuration["AzureAd:Resource"],
                    ClientId = configuration["AzureAd:ClientId"]
                }
            };
        }

        public async Task ProvisionResourcesAsync()
        {
            var accessToken = await ConsoleExtensions
                .GetTokenByCode(_config.AzureAd);
            
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
            
            var response = await client.PostAsync(_config.Endpoint, body);
            response.EnsureSuccessStatusCode();
        }
    }
}
