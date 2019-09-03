using System.Threading.Tasks;
using Windows.Storage;
using DynamicData;
using Pixama.Logic.ViewModels.Common;

namespace Pixama.Logic.Services
{
    public interface ILocationService
    {
        Task GetFolders(SourceList<FolderViewModel> foldersList);
        Task<IStorageFolder> SelectStorageFolderAsync();
        Task SaveToFavoritesAsync(IStorageFolder folder);

        Task GetDrives(SourceList<DriveViewModel> drivesList);
        Task GetChildrenFoldersAsync(IStorageFolder sourceFolder, SourceList<LocationViewModel> childrenFoldersList);
    }
}
