using Pixama.Logic.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using ReactiveUI;

namespace Pixama.Logic.ViewModels.Photo
{
    public class PhotoGridItemViewModel : BaseViewModel
    {
        #region Fields

        private readonly IPhotoService _photoService;
        private bool _isChecked;

        #endregion

        #region Properties

        public string FileName { get; }
        public string FileType { get; }
        public StorageFile StorageFile { get; }
        public DateTime DateTaken { get; private set; }
        public object Thumbnail { get; private set; }
        public bool IsChecked { get => _isChecked; set => this.RaiseAndSetIfChanged(ref _isChecked, value); }

        #endregion


        public PhotoGridItemViewModel(StorageFile storageFile, IPhotoService photoService)
        {
            _photoService = photoService;
            StorageFile = storageFile;
            FileName = storageFile.DisplayName;
            FileType = Path.GetExtension(storageFile.Path)?.Substring(1).ToUpper();
        }

        public override async Task LoadAsync()
        {
            IsLoading = true;
            Thumbnail = await _photoService.GetThumbnailAsync(StorageFile);
            DateTaken = await _photoService.GetDateTakenAsync(StorageFile);
            this.RaisePropertyChanged(nameof(Thumbnail));
            this.RaisePropertyChanged(nameof(DateTaken));
            IsLoading = false;
        }
    }
}
