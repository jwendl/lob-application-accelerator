using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Azure;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers
{
    public class AzureManager
        : IAzureManager
    {
        private readonly IConfiguration configuration;
        private readonly ILogger log;

        public AzureManager(IConfiguration configuration, ILogger log)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.log = log;
        }

        private IAzure GetAzureClient()
        {
            var loginInfo = new ServicePrincipalLoginInformation()
            {
                ClientId = configuration["ARMServicePrincipalClientId"],
                ClientSecret = configuration["ARMServicePrincipalClientSecret"]
            };

            var azureCredentials = new AzureCredentials(loginInfo, configuration["TenantId"], AzureEnvironment.AzureGlobalCloud);

            return Azure
               .Configure()
               .Authenticate(azureCredentials)
               .WithDefaultSubscription();
        }

        public async Task CreateResourceGroupIfNotExistsAsync(AzureResourceGroup resourceGroup)
        {
            log.LogInformation("Starting creation of resource group of {0}", resourceGroup.Name);

            var azure = GetAzureClient();
            var resourceGroupExists = await azure.ResourceGroups.ContainAsync(resourceGroup.Name);
            if (!resourceGroupExists)
            {
                await azure.ResourceGroups.Define(resourceGroup.Name).WithRegion(resourceGroup.Region).CreateAsync();
            }
        }

        public async Task<IDeployment> DeployARMTemplateAsync(ARMDeployment armDeployment)
        {
            log.LogInformation("Starting deployment of {0} from Uri {1}", armDeployment.Name, armDeployment.TemplateUri.AbsoluteUri);

            var azure = GetAzureClient();
            await CreateResourceGroupIfNotExistsAsync(armDeployment.ResourceGroup);

            var deployment = await azure.Deployments.Define(armDeployment.Name)
                .WithExistingResourceGroup(armDeployment.ResourceGroup.Name)
                .WithTemplateLink(armDeployment.TemplateUri.AbsoluteUri, armDeployment.TemplateContentVersion)
                .WithParameters(armDeployment.TemplateParametersJson)
                .WithMode(DeploymentMode.Complete)
                .CreateAsync();

            return deployment;
        }
    }
}
