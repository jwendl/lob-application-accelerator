using System.Threading.Tasks;

namespace LobAccelerator.Library.Interfaces
{
    public interface IOneDriveManager
    {
        Task CopyFileFromOneDriveToTeams(string teamId, string teamChannel, string originOnedrivePath);
        Task CopyFolderFromOneDriveToTeams(string teamId, string teamChannel, string originOnedriveFolder);
    }
}
