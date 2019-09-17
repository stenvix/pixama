using DynamicData;
using Pixama.Logic.ViewModels.Photo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;

namespace Pixama.Logic.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IDialogService _dialogService;

        #region Fields

        private const int BatchSize = 1000;
        private const int WarningSize = 200;

        #endregion

        public PhotoService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> GetFilesFromFolderAsync(StorageFolder storageFolder, SourceList<PhotoGridItemViewModel> items)
        {

            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, AppConstants.ImageFormats)
            {
                FolderDepth = FolderDepth.Deep,
                IndexerOption = IndexerOption.UseIndexerWhenAvailable,
            };

            SetFileTypeFilter(queryOptions.FileTypeFilter);
            var query = storageFolder.CreateFileQueryWithOptions(queryOptions);

            var filesCount = await query.GetItemCountAsync();
            if (filesCount == 0 || filesCount > WarningSize && !await _dialogService.ManyFilesDialog()) return false;
            var counter = filesCount / BatchSize + 1;
            items.Clear();

            for (int i = 0; i < counter; i++)
            {
                var firstIndex = i * BatchSize;
                var lastIndex = Math.Min(filesCount - firstIndex, BatchSize);
                var gridItems = new List<PhotoGridItemViewModel>();
                var files = await query.GetFilesAsync((uint)firstIndex, (uint)(firstIndex + lastIndex));
                foreach (var storageFile in files)
                {
                    var model = new PhotoGridItemViewModel(storageFile, this);
                    gridItems.Add(model);
                }
                //Ensure one notification per save
                items.Edit(innerList =>
                {
                    innerList.AddRange(gridItems);
                });
            }

            return true;
        }

        public async Task<BitmapImage> GetThumbnailAsync(StorageFile storageFile)
        {
            try
            {
                var thumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.PicturesView, 512);
                var bitmapSource = new BitmapImage();
                await bitmapSource.SetSourceAsync(thumbnail);
                return bitmapSource;
            }
            catch (Exception e)
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
            }
        }

        public async Task<DateTime> GetDateTakenAsync(StorageFile storageFile)
        {
            var properties = await storageFile.Properties.GetImagePropertiesAsync();
            return properties.DateTaken.DateTime;
        }


        private void SetFileTypeFilter(IList<string> filters)
        {
            foreach (var format in AppConstants.ImageFormats)
            {
                filters.Add(format);
            }
        }

    }
}
