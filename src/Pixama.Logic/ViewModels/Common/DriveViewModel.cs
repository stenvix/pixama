using DynamicData;
using Pixama.Logic.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixama.Logic.ViewModels.Common
{
    public class DriveViewModel : BaseViewModel
    {
        private string _name;
        private bool _showTree;
        private string _glyph;
        private IStorageFolder _storageFolder;
        private readonly ReadOnlyObservableCollection<DriveLocationViewModel> _children;
        private readonly SourceList<DriveLocationViewModel> _childrenFoldersList;
        private readonly IDriveService _driveService;

        public string Name { get => $"Removable Drive ({_name})"; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public string Glyph { get => _glyph; set => this.RaiseAndSetIfChanged(ref _glyph, value); }
        public bool ShowTree { get => _showTree; set => this.RaiseAndSetIfChanged(ref _showTree, value); }
        public IStorageFolder StorageFolder { get => _storageFolder; set => this.RaiseAndSetIfChanged(ref _storageFolder, value); }
        public ReadOnlyObservableCollection<DriveLocationViewModel> Children => _children;

        public DriveViewModel(IDriveService driveService)
        {
            _driveService = driveService;
            _childrenFoldersList = new SourceList<DriveLocationViewModel>();

            _childrenFoldersList.Connect()
                .Bind(out _children)
                .Subscribe();
        }

        public override async Task LoadAsync()
        {
            await _driveService.GetChildrenFoldersAsync(StorageFolder, _childrenFoldersList);
        }
    }
}
