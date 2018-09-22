using LobAccelerator.Client.Extensions;
using LobAccelerator.Client.Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Client.Models
{
    public class LobManager
    {
        public Configuration Configuration { get; }
        public IEnumerable<string> Files { get; }

        private LobManager(Options options)
        {
            Configuration = GetConfiguration(options.ConfigurationFile);
            Files = options.DefinitionsFiles;
        }

        public static Result<LobManager> Create(Options options)
        {
            if (!options.DefinitionsFiles.All(f => File.Exists(f)))
            {
                return new Result<LobManager>
                {
                    HasError = true,
                    Error = "The definition file was not found."
                };
            }
                
            if (!File.Exists(options.ConfigurationFile))
            {
                return new Result<LobManager>
                {
                    HasError = true,
                    Error = "The configuration file was not found."
                };
            }

            return new Result<LobManager>
            {
                Value = new LobManager(options)
            };
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

        public async Task<Result<None>> ProvisionResourcesAsync()
        {
            var result = new Result<None>();
            var accessToken = await ConsoleExtensions.GetTokenByCode(Configuration.AzureAd);

            try
            {
                Files.ToList().ForEach(async f => await RequestProvisioning(f, accessToken));
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Error = ex.Message;
                result.DetailedError = ex.InnerException.Message;
            }

            return result;
        }

        private async Task RequestProvisioning(string file, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Authorization", $"bearer {accessToken}");
                var content = await file.GetFileContentAsync();
                var body = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Configuration.Endpoint, body);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
