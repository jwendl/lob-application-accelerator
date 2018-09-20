using CommandLine;
using LobAccelerator.Client.Extensions;
using LobAccelerator.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

            ConsoleExtensions.DisplayInfoMessage("Sending request...");

            var manager = new LobManager(options);
            await manager.ProvisionResourcesAsync();

            ConsoleExtensions.DisplaySuccessMessage("Done!");
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
