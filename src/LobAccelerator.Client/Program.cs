using CommandLine;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobAccelerator.Client
{
    partial class Program
    {
        public const string resource = "https://graph.microsoft.com";
        public const string clientId = "398917db-d35d-4bd9-81cf-c3ff85c60e12";

        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(options => RunOptionsAndReturnExitCode(options))
                    .WithNotParsed((erros) => HandleParseError(erros));
            }
            catch (Exception ex)
            {
                DisplayErrorOnConsole(ex);
            }

#if DEBUG
            Console.ReadLine();
#endif
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            throw new NotImplementedException();
        }

        private static void RunOptionsAndReturnExitCode(Options options)
        {

            //var token = GetTokenViaCode().Result;

            //Console.WriteLine($"Your access token: {token.AccessToken}");
        }

        private static void DisplayErrorOnConsole(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something went wrong!");
            Console.WriteLine($"Message: {ex.Message}");
        }

        static async Task<AuthenticationResult> GetTokenViaCode()
        {
            var ctx = new AuthenticationContext("https://login.microsoftonline.com/common");

            DeviceCodeResult codeResult = await ctx.AcquireDeviceCodeAsync(resource, clientId);
            Console.ResetColor();
            Console.WriteLine("You need to sign in.");
            Console.WriteLine("Message: " + codeResult.Message);

            var result = await ctx.AcquireTokenByDeviceCodeAsync(codeResult);

            return result;
        }
    }
}
