using Windows.Storage;

namespace Pixama.Logic.ViewModels.Common
{
    public class StorageLocationViewModel : BaseViewModel
    {
        public string Name { get; }
        public string Glyph { get; }
        public IStorageFolder StorageFolder { get; }
        public string Path { get; }

        public StorageLocationViewModel(string name, string glyph, IStorageFolder storageFolder, string path = null)
        {
            Name = name;
            Glyph = glyph;
            StorageFolder = storageFolder;
            Path = path;
        }
    }
}
