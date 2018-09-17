using LobAccelerator.Library.Models;
using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface IWorkflowManager
    {
        Task CreateWorkflowAsync(Workflow workflow);
    }
}