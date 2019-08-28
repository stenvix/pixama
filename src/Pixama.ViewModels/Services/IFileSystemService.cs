using DynamicData;
using Pixama.ViewModels.Common;
using System.Threading.Tasks;

namespace Pixama.ViewModels.Services
{
    public interface IFileSystemService
    {
        Task GetFolders(SourceList<FolderViewModel> foldersList);
        Task GetDrives(SourceList<DriveViewModel> drivesList);
    }
}
