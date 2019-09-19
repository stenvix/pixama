using DynamicData;
using Pixama.Logic.Enums;
using Pixama.Logic.ViewModels.Common;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixama.Logic.Services
{
    public interface ILocationService
    {
        Task LoadSourceFoldersAsync(SourceList<SourceFolderViewModel> foldersList);
        Task LoadDestinationFoldersAsync(SourceList<DestinationFolderViewModel> foldersList);
        Task<StorageFolder> SelectStorageFolderAsync();
        bool SaveToFavorites(StorageFolder folder, LocationType locationType);
        bool RemoveFromFavoritesAsync(StorageFolder folder, LocationType locationType);

        Task LoadDrivesAsync(SourceList<DriveViewModel> drivesList);
        Task GetChildrenFoldersAsync(StorageFolder sourceFolder, SourceList<LocationViewModel> childrenFoldersList);

    }
}
