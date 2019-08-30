using DynamicData;
using Pixama.Logic.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.Storage;

namespace Pixama.Logic.ViewModels.Common
{
    public class DriveLocationViewModel : BaseViewModel
    {
        private string _name;
        private IStorageFolder _storageFolder;
        private readonly IDriveService _driveService;
        private readonly ReadOnlyObservableCollection<DriveLocationViewModel> _children;
        private readonly SourceList<DriveLocationViewModel> _childrenList;
        private bool _hasUnrealizedChildren;
        private bool _isLoaded;

        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public bool HasUnrealizedChildren { get => _hasUnrealizedChildren; set => this.RaiseAndSetIfChanged(ref _hasUnrealizedChildren, value); }
        public IStorageFolder StorageFolder { get => _storageFolder; set => this.RaiseAndSetIfChanged(ref _storageFolder, value); }
        public ReadOnlyObservableCollection<DriveLocationViewModel> Children => _children;

        public DriveLocationViewModel(IDriveService driveService)
        {
            _driveService = driveService;
            _childrenList = new SourceList<DriveLocationViewModel>();

            _childrenList.Connect()
                .Bind(out _children)
                .Subscribe();
        }

        public override async Task LoadAsync()
        {
            if (_isLoaded) return;
            IsLoading = true;
            await _driveService.GetChildrenFoldersAsync(StorageFolder, _childrenList);
            IsLoading = false;
            _isLoaded = true;
        }
    }
}
