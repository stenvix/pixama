using DynamicData;
using Pixama.ViewModels.Common;
using Pixama.ViewModels.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Pixama.ViewModels.Photo
{
    public class PhotoViewModel : BaseViewModel
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly SourceList<DriveViewModel> _drivesList;
        private readonly SourceList<FolderViewModel> _foldersList;
        private readonly ReadOnlyObservableCollection<DriveViewModel> _drives;
        private readonly ReadOnlyObservableCollection<FolderViewModel> _folders;
        private DriveViewModel _selectedDrive;
        private FolderViewModel _selectedFolder;

        public ReadOnlyObservableCollection<DriveViewModel> Drives => _drives;
        public ReadOnlyObservableCollection<FolderViewModel> Folders => _folders;

        public DriveViewModel SelectedDrive
        {
            get => _selectedDrive;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDrive, value);
                this.RaiseAndSetIfChanged(ref _selectedFolder, null, nameof(SelectedFolder));
            }
        }

        public FolderViewModel SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFolder, value);
                this.RaiseAndSetIfChanged(ref _selectedDrive, null, nameof(SelectedDrive));
            }
        }

        public PhotoViewModel(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _drivesList = new SourceList<DriveViewModel>();
            _foldersList = new SourceList<FolderViewModel>();

            _drivesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _drives)
                .Subscribe();

            _foldersList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _folders)
                .Subscribe();
        }

        public override async Task LoadAsync()
        {
            IsLoading = true;
            await _fileSystemService.GetFolders(_foldersList);
            await _fileSystemService.GetDrives(_drivesList);
            IsLoading = false;
        }
    }
}
