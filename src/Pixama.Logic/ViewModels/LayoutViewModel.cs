using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Photo;
using ReactiveUI;

namespace Pixama.Logic.ViewModels
{
    public class LayoutViewModel : BaseViewModel
    {
        #region Fields

        private readonly INavigationService _navigationService;
        private LayoutMenuItemViewModel _selectedMenuItem;

        private static readonly ObservableCollection<LayoutMenuItemViewModel> MenuItems = new ObservableCollection<LayoutMenuItemViewModel>
        {
            new LayoutMenuItemViewModel("Photos",typeof(PhotoViewModel))
        };

        #endregion

        #region Constructors

        public LayoutViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.WhenAnyValue(i => i.SelectedMenuItem)
                .Where(i => i != null)
                .Select(i => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(this.Navigate));
        }

        #endregion

        #region Properties

        public ObservableCollection<LayoutMenuItemViewModel> LayoutMenuItems => MenuItems;

        public LayoutMenuItemViewModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => this.RaiseAndSetIfChanged(ref _selectedMenuItem, value);
        }

        #endregion

        #region Methods

        private async Task Navigate()
        {
            await this._navigationService.NavigateTo(this.SelectedMenuItem.ViewModel);
        }

        #endregion
    }
}
