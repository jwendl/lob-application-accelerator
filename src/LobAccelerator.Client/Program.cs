using CommandLine;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobAccelerator.Client
{
    class Program
    {
        public const string resource = "https://graph.microsoft.com";
        public const string clientId = "398917db-d35d-4bd9-81cf-c3ff85c60e12";

        class Options
        {
            [Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
            public IEnumerable<string> InputFiles { get; set; }

            // Omitting long name, defaults to name of property, ie "--verbose"
            [Option(
              Default = false,
              HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }

            [Option("stdin",
              Default = false,
              HelpText = "Read from stdin")]
            public bool stdin { get; set; }

        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");

                CommandLine.Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
                    .WithNotParsed<Options>((errs) => HandleParseError(errs));
            }
            catch (Exception ex)
            {
                DisplayErrorOnConsole(ex);
            }

            Console.ReadLine();
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            throw new NotImplementedException();
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            foreach (var item in opts.InputFiles)
            {
                Console.WriteLine(item);
            }

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
