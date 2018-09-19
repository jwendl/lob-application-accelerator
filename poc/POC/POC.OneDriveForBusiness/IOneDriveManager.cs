using System.Threading.Tasks;

namespace POC.OneDriveForBusiness
{
    public interface IOneDriveManager
    {
        Task CopyFileFromOneDriveToTeams(string teamId, string originOnedrivePath);
        Task CopyFolderFromOneDriveToTeams(string teamId, string originOnedriveFolder);
    }
}