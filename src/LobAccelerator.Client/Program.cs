using CommandLine;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LobAccelerator.Client
{
    partial class Program
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
                DisplayErrorOnConsole(ex);
            }

#if DEBUG
            Console.ReadLine();
#endif
        }

        private static void DisplayErrorOnConsole(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something went wrong!");
            Console.WriteLine($"Message: {ex.Message}");
        }

        private static void DisplayInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
        }

        private static void DisplaySuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
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

            DisplayInfoMessage("Input validated...");

            var token = await GetTokenViaCode(resource, clientId);
            string accessToken = token.AccessToken;

            DisplayInfoMessage("Token acquired...");

            DisplayInfoMessage("Sending request...");

            await SendFilesToEngine(url, accessToken, files);

            DisplaySuccessMessage("Finished!");
        }


        private static void ValidateInput(Options options)
        {
            if (!options.DefinitionsFiles.All(f => File.Exists(f)))
                throw new FileNotFoundException("A definition file was not found.");

            if (!File.Exists(options.ConfigurationFile))
                throw new FileNotFoundException("The configuration file was not found.");
        }

        static async Task<AuthenticationResult> GetTokenViaCode(string resource, string clientId)
        {
            var ctx = new AuthenticationContext("https://login.microsoftonline.com/common");

            DeviceCodeResult codeResult = await ctx.AcquireDeviceCodeAsync(resource, clientId);
            Console.ResetColor();
            Console.WriteLine("You need to sign in.");
            Console.WriteLine("Message: " + codeResult.Message);

            return await ctx.AcquireTokenByDeviceCodeAsync(codeResult);
        }

        static async Task SendFilesToEngine(string url, string accessToken, IEnumerable<string> files)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (var file in files)
                {
                    var content = await File.ReadAllTextAsync(file);
                    var body = new StringContent(content, Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Add("X-Authorization", $"bearer {accessToken}");

                    var response = await httpClient.PostAsync(url, body);
                    response.EnsureSuccessStatusCode();
                }
            }

        }
    }
}
