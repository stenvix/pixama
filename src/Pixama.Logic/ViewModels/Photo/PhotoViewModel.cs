using DynamicData;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Common;
using Pixama.Logic.ViewModels.Events;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Input;

namespace Pixama.Logic.ViewModels.Photo
{
    public class PhotoViewModel : BaseViewModel
    {
        #region Fields

        private readonly ILocationService _locationService;
        private readonly DeviceWatcher _devicesWatcher;
        private readonly ObservableAsPropertyHelper<bool> _hasRemovableDrives;
        private readonly SourceList<DriveViewModel> _drivesList;
        private readonly SourceList<FolderViewModel> _foldersList;
        private readonly ReadOnlyObservableCollection<DriveViewModel> _drives;
        private readonly ReadOnlyObservableCollection<FolderViewModel> _folders;
        private BaseLocationViewModel _selectedLocation;
        private bool _isLoaded;

        #endregion

        #region Properties

        public bool HasRemovableDrive => _hasRemovableDrives.Value;
        public ReadOnlyObservableCollection<DriveViewModel> Drives => _drives;
        public ReadOnlyObservableCollection<FolderViewModel> Folders => _folders;

        public BaseLocationViewModel SelectedLocation { get => _selectedLocation; set => this.RaiseAndSetIfChanged(ref _selectedLocation, value); }

        public ReactiveCommand<PointerRoutedEventArgs, Unit> AddFolderCommand { get; }

        #endregion

        public PhotoViewModel(ILocationService locationService)
        {
            _locationService = locationService;
            _drivesList = new SourceList<DriveViewModel>();
            _foldersList = new SourceList<FolderViewModel>();
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

            AddFolderCommand = ReactiveCommand.CreateFromTask<PointerRoutedEventArgs>(AddFolderAsync);
            MessageBus.Current.Listen<LocationRemoved>().Subscribe(OnLocationRemoved);
        }

        public override async Task LoadAsync()
        {
            if (_isLoaded) return;
            IsLoading = true;
            await _locationService.LoadFoldersAsync(_foldersList);
            _isLoaded = true;
            IsLoading = false;
        }

        private async Task AddFolderAsync(PointerRoutedEventArgs args)
        {
            var folder = await _locationService.SelectStorageFolderAsync();
            if (folder == null) return;
            if (!_locationService.SaveToFavorites(folder)) return; //Todo: show duplication message box
            await _locationService.LoadFolderAsync(folder, _foldersList);
        }

        public void StartDevicesTracking()
        {
            _devicesWatcher.Start();
        }

        private async void OnDeviceChanged(DeviceWatcher sender, object args)
        {
            await _locationService.LoadDrivesAsync(_drivesList);
        }

        public void StopDevicesTracking()
        {
            _devicesWatcher.Stop();
        }

        private void OnLocationRemoved(LocationRemoved args)
        {
            _foldersList.Remove(args.Location);
        }
    }
}
