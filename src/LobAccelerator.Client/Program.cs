using CommandLine;
using LobAccelerator.Client.Extensions;
using LobAccelerator.Client.Models;
using LobAccelerator.Client.Models.Common;
using System;
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
                    .WithParsed(async options => {
                        await RunOptionAsync(options);
                    });
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
            try
            {
                ConsoleExtensions.DisplayInfoMessage("Sending request...");

                Result<LobManager> managerResult = LobManager.Create(options);
                Result<None> provisionResult = await managerResult.Value.ProvisionResourcesAsync();

                var result = Result.Combine(managerResult, provisionResult);

                if (result.HasError())
                {
                    throw new InvalidOperationException(managerResult.Error);
                }

                ConsoleExtensions.DisplaySuccessMessage("Done!");
            }
            catch (Exception ex)
            {
                var t = ex.Message;
            }
        }
    }
}
