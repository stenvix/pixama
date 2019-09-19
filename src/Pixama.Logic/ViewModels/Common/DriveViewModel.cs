using Pixama.Logic.Services;
using System.Threading.Tasks;

namespace Pixama.Logic.ViewModels.Common
{
    public class DriveViewModel : BaseLocationViewModel
    {
        private bool _isLoaded;
        public override string ExpandGlyph => GetExpandGlyph();

        public DriveViewModel(ILocationService locationService) : base(locationService)
        {
        }

        public override async Task LoadAsync()
        {
            if (_isLoaded) return;
            IsLoading = true;
            await LocationService.GetChildrenFoldersAsync(StorageFolder, ChildrenList);
            HasUnrealizedChildren = ChildrenList.Count != 0;
            IsLoading = false;
            _isLoaded = true;
        }

        private string GetExpandGlyph()
        {
            if (IsExpanded) return "\uE70E";
            return "\uE70D";
        }
    }
}
