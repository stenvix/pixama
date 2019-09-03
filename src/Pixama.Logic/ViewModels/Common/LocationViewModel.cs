using Pixama.Logic.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Pixama.Logic.ViewModels.Common
{
    public class LocationViewModel : BaseLocationViewModel
    {
        private bool _isLoaded;

        public override string ExpandGlyph => GetExpandGlyph();

        public LocationViewModel(ILocationService locationService) : base(locationService)
        {
            this.WhenAnyValue(i => i.IsExpanded)
                .Where(i => i)
                .InvokeCommand(ReactiveCommand.CreateFromTask<bool>(async _ => await LoadAsync()));
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
            if (IsExpanded) return "\uE70D";
            return "\uE76C";
        }
    }
}
