using DynamicData;
using Pixama.Logic.ViewModels.Common;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixama.Logic.Services
{
    public interface IDriveService
    {
        Task GetDrives(SourceList<DriveViewModel> drivesList);
        Task GetChildrenFoldersAsync(IStorageFolder sourceFolder, SourceList<DriveLocationViewModel> childrenFoldersList);
        Task<bool> HasChildFoldersAsync(StorageFolder sourceFolder);
    }
}
