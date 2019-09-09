using DynamicData;
using Pixama.Logic.ViewModels.Photo;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Pixama.Logic.Services
{
    public interface IPhotoService
    {
        Task GetFilesFromFolderAsync(StorageFolder storageFolder, SourceList<PhotoGridItemViewModel> items);
        Task<BitmapImage> GetThumbnailAsync(StorageFile storageFile);
        Task<DateTime> GetDateTakenAsync(StorageFile storageFile);
    }
}
