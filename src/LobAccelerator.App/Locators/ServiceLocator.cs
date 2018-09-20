using LobAccelerator.Library.Configuration;
using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers;
using LobAccelerator.Library.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace LobAccelerator.App.Locators
{
    public static class ServiceLocator
    {
        private static readonly IServiceProvider serviceProvider;

        static ServiceLocator()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<HttpClient, HttpClient>();
            serviceCollection.AddSingleton<IWorkflowManager, WorkflowManager>((sp) =>
            {
                return new WorkflowManager("eyJ0eXAiOiJKV1QiLCJub25jZSI6IkFRQUJBQUFBQUFEWHpaM2lmci1HUmJEVDQ1ek5TRUZFVVlXa042TExkUFlyQTF6eTNlU1puYUdFeHk5Z25qNGVNSzV5LUdoUjdnTHhPeF9aNVl0bUY0VDlhM2NWVGZDbjBFRXo5Q3JtVmt0NWJYSU1FUXVQOXlBQSIsImFsZyI6IlJTMjU2IiwieDV0IjoiaTZsR2szRlp6eFJjVWIyQzNuRVE3c3lISmxZIiwia2lkIjoiaTZsR2szRlp6eFJjVWIyQzNuRVE3c3lISmxZIn0.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20vIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvNDUzYjg5MjEtYmU0NC00OWVjLWJkM2MtODc0MTRkYjEwY2IyLyIsImlhdCI6MTUzNzQwMTIyOCwibmJmIjoxNTM3NDAxMjI4LCJleHAiOjE1Mzc0MDUxMjgsImFjY3QiOjAsImFjciI6IjEiLCJhaW8iOiJBU1FBMi84SUFBQUFpVjc4OExBV1N1SVZiSnJPYlVmUS9ZWG5wSDV0SmxEUWVHZlE1ZXBFMXo4PSIsImFtciI6WyJwd2QiXSwiYXBwX2Rpc3BsYXluYW1lIjoiTE9CIEFjY2VsZXJhdG9yIiwiYXBwaWQiOiIzOTg5MTdkYi1kMzVkLTRiZDktODFjZi1jM2ZmODVjNjBlMTIiLCJhcHBpZGFjciI6IjEiLCJmYW1pbHlfbmFtZSI6IlVzZXIiLCJnaXZlbl9uYW1lIjoiQWRtaW4iLCJpcGFkZHIiOiIxMzEuMTA3LjE1OS45NyIsIm5hbWUiOiJBZG1pbiBVc2VyIiwib2lkIjoiNmVhMTU2ZDktMDNjMC00NjNlLWI0NDMtMTdjODUzOGJhYThiIiwicGxhdGYiOiIzIiwicHVpZCI6IjEwMDM3RkZFQUUwMzdEMjQiLCJzY3AiOiJBbGxTaXRlcy5GdWxsQ29udHJvbCBEaXJlY3RvcnkuQWNjZXNzQXNVc2VyLkFsbCBEaXJlY3RvcnkuUmVhZC5BbGwgRGlyZWN0b3J5LlJlYWRXcml0ZS5BbGwgR3JvdXAuUmVhZFdyaXRlLkFsbCBvZmZsaW5lX2FjY2VzcyBvcGVuaWQgU2l0ZXMuUmVhZFdyaXRlLkFsbCBVc2VyLlJlYWQgVXNlci5SZWFkQmFzaWMuQWxsIiwic3ViIjoiSExLakd1Z3g5QTNaLXZzc0FOMDBoWXFvOWFGazRGZHJpWVJqMmxwX0tYNCIsInRpZCI6IjQ1M2I4OTIxLWJlNDQtNDllYy1iZDNjLTg3NDE0ZGIxMGNiMiIsInVuaXF1ZV9uYW1lIjoiYWRtaW5AandhenVyZWFkLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6ImFkbWluQGp3YXp1cmVhZC5vbm1pY3Jvc29mdC5jb20iLCJ1dGkiOiI0MGRyMDdfRjAwaThvcmUzc2dVSkFBIiwidmVyIjoiMS4wIiwid2lkcyI6WyI2MmU5MDM5NC02OWY1LTQyMzctOTE5MC0wMTIxNzcxNDVlMTAiXSwieG1zX3RjZHQiOiIxNTM3MjA4ODA1In0.HUcpdA69Qaa1e-cXFlXuGYS3UsaAJIPSlgzoaPkqNnZvJ8HpZnHfEhEPGfvD1XZg031GHG-zRx1_0uEr5PBZ3QgpRmha6xy1_wmRUeSJy__uKgdAK1ER5IniqAmVvPgVseVvqXyPASbrSw0x0YNfgOq32SfU6cgdHGfQi6XZAZ2N-Dfs1jVqT_X2gfmTK8vz_gVYsg3jiIEf4-1qN7Zx9UWZ5Oe6Pj59GoNMXRjE8vifhdy7LIhGWVUMGL8haCBOSlq0S2JPvnjXwN2SGGjnpVex-Sm8qXANLVr1tnzfxWrw90sbPguRSE4TMNKAkAPNiCL4S5bQeZEK1M_LLtCUEg");
            });
            //serviceCollection.AddSingleton<IConfiguration, ConfigurationManager>();
            //serviceCollection.AddSingleton<ITokenManager, TokenManager>();
            //serviceCollection.AddSingleton<ITokenCacheHelper, TokenCacheHelper>();

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public static TInterface GetRequiredService<TInterface>()
        {
            return serviceProvider.GetRequiredService<TInterface>();
        }
    }
}
