using Pixama.Logic.Services;
using System.Threading.Tasks;

namespace Pixama.Logic.ViewModels.Common
{
    public class DriveViewModel : BaseLocationViewModel
    {
        public override string ExpandGlyph => GetExpandGlyph();

        public DriveViewModel(ILocationService locationService) : base(locationService)
        {
        }

        public override async Task LoadAsync()
        {
            IsLoading = true;
            await LocationService.GetChildrenFoldersAsync(StorageFolder, ChildrenList);
            HasUnrealizedChildren = ChildrenList.Count != 0;
            IsLoading = false;
        }

        private string GetExpandGlyph()
        {
            if (IsExpanded) return "\uE70E";
            return "\uE70D";
        }
    }
}
