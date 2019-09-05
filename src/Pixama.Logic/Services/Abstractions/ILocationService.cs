using DynamicData;
using Pixama.Logic.ViewModels.Common;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixama.Logic.Services
{
    public interface ILocationService
    {
        Task LoadFoldersAsync(SourceList<FolderViewModel> foldersList);
        Task LoadFolderAsync(StorageFolder storageFolder, SourceList<FolderViewModel> foldersList);
        Task<StorageFolder> SelectStorageFolderAsync();
        bool SaveToFavorites(StorageFolder folder);
        bool RemoveFromFavoritesAsync(StorageFolder folder);

        Task LoadDrivesAsync(SourceList<DriveViewModel> drivesList);
        Task GetChildrenFoldersAsync(StorageFolder sourceFolder, SourceList<LocationViewModel> childrenFoldersList);
    }
}
