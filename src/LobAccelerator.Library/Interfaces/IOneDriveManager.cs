using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface IOneDriveManager
    {
        Task CopyFileFromOneDriveToTeams(string teamId, string originOnedrivePath);
        Task CopyFolderFromOneDriveToTeams(string teamId, string originOnedriveFolder);
    }
}
