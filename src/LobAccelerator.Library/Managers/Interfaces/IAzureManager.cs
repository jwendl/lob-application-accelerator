using LobAccelerator.Library.Models.Azure;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface IAzureManager
    {
        Task CreateResourceGroupIfNotExistsAsync(AzureResourceGroup resourceGroup);
        Task<IDeployment> DeployARMTemplateAsync(ARMDeployment armDeployment);
    }
}
