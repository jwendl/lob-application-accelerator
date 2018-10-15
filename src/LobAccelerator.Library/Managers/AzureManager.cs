using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Managers.Interfaces;
using LobAccelerator.Library.Models.Azure;
using LobAccelerator.Library.Models.Common;
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

            var azureCredentials = new AzureCredentials(loginInfo,
                configuration["TenantId"],
                AzureEnvironment.AzureGlobalCloud);

            return Azure
               .Configure()
               .Authenticate(azureCredentials)
               .WithDefaultSubscription();
        }

        public async Task<IResult> CreateResourceGroupIfNotExistsAsync(AzureResourceGroup resourceGroup)
        {
            try
            {
                log.LogInformation("Starting creation of resource group of {0}",
                    resourceGroup.Name);

                var azure = GetAzureClient();

                var rgExists = await azure.ResourceGroups.ContainAsync(resourceGroup.Name);
                if (!rgExists)
                {
                    await azure.ResourceGroups.Define(resourceGroup.Name).WithRegion(resourceGroup.Region).CreateAsync();
                }

                var result = new Result<bool>();
                result.Value = true;
                return result;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Exception encountered during creation of resource group {resourceGroup.Name}: {ex.GetType()} - {ex.Message}";
                log.LogError(errorMsg);

                var result = new Result<bool>();
                result.HasError = true;
                result.Error = errorMsg;
                return result;
            }
        }

        public async Task<IResult> DeployARMTemplateAsync(ARMDeployment armDeployment)
        {
            try
            {
                log.LogInformation("Starting deployment of {0} from Uri {1}",
                    armDeployment.Name,
                    armDeployment.TemplateUri.AbsoluteUri);

                var azure = GetAzureClient();

                var rgResult = await CreateResourceGroupIfNotExistsAsync(armDeployment.ResourceGroup);
                if (rgResult.HasError()) return rgResult;

                var deployment = await azure.Deployments.Define(armDeployment.Name)
                    .WithExistingResourceGroup(armDeployment.ResourceGroup.Name)
                    .WithTemplateLink(armDeployment.TemplateUri.AbsoluteUri, armDeployment.TemplateContentVersion)
                    .WithParameters(armDeployment.TemplateParametersJson)
                    .WithMode(DeploymentMode.Complete)
                    .CreateAsync();

                var result = new Result<IDeployment>();
                result.Value = deployment;
                return result;
            }
            catch (Exception ex)
            {
                var errorMsg =
                    $"Exception encountered during ARM deployment of {armDeployment.Name} from uri {armDeployment.TemplateUri.AbsoluteUri}: {ex.GetType()} - {ex.Message}";
                log.LogError(errorMsg);

                var result = new Result<IDeployment>();
                result.HasError = true;
                result.Error = errorMsg;
                return result;
            }
        }
    }
}
