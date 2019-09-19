using DynamicData;
using Pixama.Logic.Enums;
using Pixama.Logic.Helpers;
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
using Windows.Storage;
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
        private readonly SourceList<SourceFolderViewModel> _sourceFoldersList;
        private readonly SourceList<DestinationFolderViewModel> _destinationFoldersList;
        private readonly ReadOnlyObservableCollection<DriveViewModel> _drives;
        private readonly ReadOnlyObservableCollection<SourceFolderViewModel> _sourceFolders;
        private readonly ReadOnlyObservableCollection<DestinationFolderViewModel> _destinationFolders;
        private BaseLocationViewModel _selectedLocation;
        private bool _isLoaded;

        #endregion

        #region Properties

        public bool HasRemovableDrive => _hasRemovableDrives.Value;
        public ReadOnlyObservableCollection<DriveViewModel> Drives => _drives;
        public ReadOnlyObservableCollection<SourceFolderViewModel> SourceFolders => _sourceFolders;
        public ReadOnlyObservableCollection<DestinationFolderViewModel> DestinationFolders => _destinationFolders;

        public BaseLocationViewModel SelectedLocation { get => _selectedLocation; set => this.RaiseAndSetIfChanged(ref _selectedLocation, value); }

        public ReactiveCommand<TappedRoutedEventArgs, Unit> AddSourceFolderCommand { get; }
        public ReactiveCommand<TappedRoutedEventArgs, Unit> AddDestinationFolderCommand { get; }

        #endregion

        public PhotoViewModel(ILocationService locationService)
        {
            _locationService = locationService;
            _drivesList = new SourceList<DriveViewModel>();
            _sourceFoldersList = new SourceList<SourceFolderViewModel>();
            _destinationFoldersList = new SourceList<DestinationFolderViewModel>();
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

            _sourceFoldersList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _sourceFolders)
                .Subscribe();

            _destinationFoldersList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _destinationFolders)
                .Subscribe();

            AddSourceFolderCommand = ReactiveCommand.CreateFromTask<TappedRoutedEventArgs>(AddSourceFolderAsync);
            AddDestinationFolderCommand = ReactiveCommand.CreateFromTask<TappedRoutedEventArgs>(AddDestinationFolderAsync);
            MessageBus.Current.Listen<SourceLocationRemoved>().Subscribe(OnSourceLocationRemoved);
            MessageBus.Current.Listen<DestinationLocationRemoved>().Subscribe(OnDestinationLocationRemoved);
        }

        public override async Task LoadAsync()
        {
            if (_isLoaded) return;
            IsLoading = true;
            await _locationService.LoadSourceFoldersAsync(_sourceFoldersList);
            await _locationService.LoadDestinationFoldersAsync(_destinationFoldersList);
            _isLoaded = true;
            IsLoading = false;
        }

        private async Task AddSourceFolderAsync(TappedRoutedEventArgs args)
        {
            var storageFolder = await _locationService.SelectStorageFolderAsync();
            if (storageFolder == null) return;
            if (!_locationService.SaveToFavorites(storageFolder, LocationType.Source)) return; //Todo: show duplication message box
            AddSourceFolderToList(storageFolder);
        }

        private async Task AddDestinationFolderAsync(TappedRoutedEventArgs args)
        {
            var folder = await _locationService.SelectStorageFolderAsync();
            if (folder == null) return;
            if (!_locationService.SaveToFavorites(folder, LocationType.Destination)) return;
            AddDestinationFolderToList(folder);
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

        private void OnSourceLocationRemoved(SourceLocationRemoved args)
        {
            _sourceFoldersList.Remove(args.Location);
        }

        private void OnDestinationLocationRemoved(DestinationLocationRemoved args)
        {
            _destinationFoldersList.Remove(args.Location);
        }

        private void AddSourceFolderToList(StorageFolder storageFolder)
        {
            var model = new SourceFolderViewModel(_locationService)
            {
                Name = storageFolder.DisplayName,
                Glyph = Glyphs.FolderGlyph,
                StorageFolder = storageFolder
            };
            _sourceFoldersList.Add(model);
        }

        private void AddDestinationFolderToList(StorageFolder storageFolder)
        {
            var model = new DestinationFolderViewModel(_locationService)
            {
                Name = storageFolder.DisplayName,
                Glyph = Glyphs.FolderGlyph,
                StorageFolder = storageFolder
            };
            _destinationFoldersList.Add(model);
        }
    }
}
