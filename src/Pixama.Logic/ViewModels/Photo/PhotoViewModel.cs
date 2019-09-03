using DynamicData;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Common;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Pixama.Logic.ViewModels.Photo
{
    public class PhotoViewModel : BaseViewModel
    {
        #region Fields

        private readonly IFolderService _folderService;
        private readonly IDriveService _driveService;
        private readonly DeviceWatcher _devicesWatcher;
        private readonly ObservableAsPropertyHelper<bool> _hasRemovableDrives;
        private readonly SourceList<DriveViewModel> _drivesList;
        private readonly SourceList<BaseLocationViewModel> _foldersList;
        private readonly ReadOnlyObservableCollection<DriveViewModel> _drives;
        private readonly ReadOnlyObservableCollection<BaseLocationViewModel> _folders;
        private StorageLocationViewModel _selectedLocation;

        public ReactiveCommand<Unit, Unit> AddFolderCommand;
        private BaseLocationViewModel _selectedLocation1;

        #endregion

        #region Properties

        public bool HasRemovableDrive => _hasRemovableDrives.Value;
        public ReadOnlyObservableCollection<DriveViewModel> Drives => _drives;
        public ReadOnlyObservableCollection<BaseLocationViewModel> Folders => _folders;

        public BaseLocationViewModel SelectedLocation1 { get => _selectedLocation1; set => this.RaiseAndSetIfChanged(ref _selectedLocation1, value); }
        public StorageLocationViewModel SelectedLocation { get => _selectedLocation; set => this.RaiseAndSetIfChanged(ref _selectedLocation, value); }

        #endregion

        public PhotoViewModel(IFolderService folderService, IDriveService driveService)
        {
            _folderService = folderService;
            _driveService = driveService;
            _drivesList = new SourceList<DriveViewModel>();
            _foldersList = new SourceList<BaseLocationViewModel>();
            _devicesWatcher = DeviceInformation.CreateWatcher(DeviceClass.PortableStorageDevice);
            _devicesWatcher.Added += OnDeviceChanged;
            _devicesWatcher.Removed += OnDeviceChanged;

            _drivesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _drives)
                .Subscribe();

            _drivesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .AsObservableList()
                .CountChanged
                .Select(i => i != 0)
                .ToProperty(this, vm => vm.HasRemovableDrive, out _hasRemovableDrives);

            _foldersList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _folders)
                .Subscribe();

            AddFolderCommand = ReactiveCommand.CreateFromTask(AddFolder);
        }

        public override async Task LoadAsync()
        {
            IsLoading = true;
            await _folderService.GetFolders(_foldersList);
            IsLoading = false;
        }

        private async Task AddFolder()
        {
            var folder = await _folderService.SelectStorageFolderAsync();
            if (folder == null) return;
            await _folderService.SaveToFavoritesAsync(folder);
            await _folderService.GetFolders(_foldersList);
        }

        public void StartDevicesTracking()
        {
            _devicesWatcher.Start();
        }

        private async void OnDeviceChanged(DeviceWatcher sender, object args)
        {
            await _driveService.GetDrives(_drivesList);
        }

        public void StopDevicesTracking()
        {
            _devicesWatcher.Stop();
        }
    }
}
