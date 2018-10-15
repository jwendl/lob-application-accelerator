using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models.Azure;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Managers.Interfaces
{
    public interface IAzureManager
    {
        Task<IResult> CreateResourceGroupIfNotExistsAsync(AzureResourceGroup resourceGroup);
        Task<IResult> DeployARMTemplateAsync(ARMDeployment armDeployment);
    }
}
