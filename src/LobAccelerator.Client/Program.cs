using CommandLine;
using LobAccelerator.Client.Extensions;
using LobAccelerator.Client.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace LobAccelerator.Client
{
    public static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(options => RunOptions(options));
            }
            catch (Exception ex)
            {
                ConsoleExtensions.DisplayError(ex);
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
        
        private static void RunOptions(Options options)
        {
            RunOptionAsync(options).GetAwaiter().GetResult();
        }

        private static async Task RunOptionAsync(Options options)
        {
            ValidateInput(options);

            var configuration = new ConfigurationManager(options.ConfigurationFile);

            IEnumerable<string> files = options.DefinitionsFiles;
            string resource = configuration["AzureAd:Resource"];
            string clientId = configuration["AzureAd:ClientId"];
            string url = configuration["LobEngine:Endpoint"];

            ConsoleExtensions.DisplayInfoMessage("Input validated...");

            var accessToken = await ConsoleExtensions.GetTokenByCode(resource, clientId);

            ConsoleExtensions.DisplayInfoMessage("Token acquired...");
            ConsoleExtensions.DisplayInfoMessage("Sending request...");

            var manager = new LobManager();
            await manager.ProvisionResourcesAsync(url, accessToken, files);

            ConsoleExtensions.DisplaySuccessMessage("Finished!");
        }
        
        private static void ValidateInput(Options options)
        {
            if (!options.DefinitionsFiles.All(f => File.Exists(f)))
                throw new FileNotFoundException("A definition file was not found.");

            if (!File.Exists(options.ConfigurationFile))
                throw new FileNotFoundException("The configuration file was not found.");
        }
        
        
    }
}
