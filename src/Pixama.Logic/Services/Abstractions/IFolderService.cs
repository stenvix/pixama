using System.Threading.Tasks;
using Windows.Storage;
using DynamicData;
using Pixama.Logic.ViewModels.Common;

namespace Pixama.Logic.Services
{
    public interface IFolderService
    {
        Task GetFolders(SourceList<BaseLocationViewModel> foldersList);
        Task<IStorageFolder> SelectStorageFolderAsync();
        Task SaveToFavoritesAsync(IStorageFolder folder);
    }
}
