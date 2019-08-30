using System;
using System.Threading.Tasks;
using Windows.Storage;
using DynamicData;
using ReactiveUI;

namespace Pixama.Logic.ViewModels.Common
{
    public class StorageLocationViewModel : BaseViewModel
    {
        private bool _showTree;
        private readonly SourceList<StorageLocationViewModel> _childrenFoldersList;
        public string Name { get; }
        public string Glyph { get; }
        public IStorageFolder StorageFolder { get; }
        public string Path { get; }

        public bool ShowTree
        {
            get => _showTree;
            set => this.RaiseAndSetIfChanged(ref _showTree, value);
        }

        public StorageLocationViewModel(string name, string glyph, IStorageFolder storageFolder, string path = null)
        {
            Name = name;
            Glyph = glyph;
            StorageFolder = storageFolder;
            Path = path;
            _childrenFoldersList = new SourceList<StorageLocationViewModel>();
        }

        public override async Task LoadAsync()
        {
            await LoadChildrenFolders();
        }

        private async Task LoadChildrenFolders()
        {
            var folders = await StorageFolder.GetFoldersAsync();
            _childrenFoldersList.Edit(list =>
            {

            });
        }
    }
}
