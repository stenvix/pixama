using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Events;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Pixama.Logic.ViewModels.Common
{
    public class FolderViewModel : BaseLocationViewModel
    {

        #region Fields

        private bool _isStatic;
        private readonly ObservableAsPropertyHelper<bool> _isDynamic;

        #endregion

        #region Properties

        public override string ExpandGlyph => GetExpandGlyph();
        public bool IsDynamic => _isDynamic.Value;
        public bool IsStatic { get => _isStatic; set => this.RaiseAndSetIfChanged(ref _isStatic, value); }
        public ReactiveCommand<Unit, Unit> FavoriteClickCommand { get; }

        #endregion

        public FolderViewModel(ILocationService locationService) : base(locationService)
        {
            FavoriteClickCommand = ReactiveCommand.CreateFromTask<Unit>(OnFavoriteClick);
            this.WhenAnyValue(i => i.IsStatic)
                .Select(i => !i)
                .ToProperty(this, i => i.IsDynamic, out _isDynamic);
        }

        public override async Task LoadAsync()
        {
            IsLoading = true;
            await LocationService.GetChildrenFoldersAsync(StorageFolder, ChildrenList);
            HasUnrealizedChildren = ChildrenList.Count != 0;
            IsLoading = false;
        }

        private async Task OnFavoriteClick(Unit unit)
        {
            if (!LocationService.RemoveFromFavoritesAsync(StorageFolder)) return; //Todo: show warning message
            MessageBus.Current.SendMessage(new LocationRemoved(this));
        }

        private string GetExpandGlyph()
        {
            if (IsExpanded) return "\uE70E";
            return "\uE70D";
        }
    }
}
