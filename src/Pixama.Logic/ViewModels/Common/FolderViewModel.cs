using Pixama.Logic.Services;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace Pixama.Logic.ViewModels.Common
{
    public class FolderViewModel : BaseLocationViewModel
    {
        #region Fields

        private bool _isStatic;
        private ObservableAsPropertyHelper<bool> _isDynamic;

        #endregion

        #region Properties

        public override string ExpandGlyph => GetExpandGlyph();
        public bool IsDynamic => _isDynamic.Value;
        public bool IsStatic { get => _isStatic; set => this.RaiseAndSetIfChanged(ref _isStatic, value); }
        public ReactiveCommand<PointerRoutedEventArgs, Unit> FavoriteClickCommand { get; }

        #endregion

        public FolderViewModel(ILocationService locationService) : base(locationService)
        {
            FavoriteClickCommand = ReactiveCommand.CreateFromTask<PointerRoutedEventArgs, Unit>(OnFavoriteClick);
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

        private Task<Unit> OnFavoriteClick(PointerRoutedEventArgs arg)
        {
            return Task.FromResult(Unit.Default);
        }

        private string GetExpandGlyph()
        {
            if (IsExpanded) return "\uE70E";
            return "\uE70D";
        }
    }
}
