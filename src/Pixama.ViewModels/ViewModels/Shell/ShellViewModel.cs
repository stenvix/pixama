using Pixama.ViewModels.Services;
using Pixama.ViewModels.ViewModels.Photo;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Pixama.ViewModels.ViewModels.Shell
{
    public class ShellViewModel : BaseViewModel
    {
        #region Fields

        private ShellMenuItemViewModel _selectedTab;
        private readonly INavigationService _navigation;

        private readonly ShellMenuItemViewModel _photoMenuItem = new ShellMenuItemViewModel(typeof(PhotoViewModel)) { Name = "Photos" };
        private readonly ShellMenuItemViewModel _videoMenuItem = new ShellMenuItemViewModel(typeof(VideoViewModel)) { Name = "Videos" };

        #endregion

        #region Properties

        public ObservableCollection<ShellMenuItemViewModel> MenuItems { get; set; }
        public ShellMenuItemViewModel SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }

        #endregion

        public ShellViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            MenuItems = GetMenuItems();
            this.WhenAnyValue(i => i.SelectedTab)
                .Where(i => i != null)
                .Select(i => Unit.Default)
                .InvokeCommand(ReactiveCommand.CreateFromTask(OnSelectedTabChanged));
        }

        private async Task OnSelectedTabChanged()
        {
            switch (SelectedTab.Name)
            {
                case "Photos":
                    {
                        await _navigation.NavigateTo<PhotoViewModel>();
                        break;
                    }
                case "Videos":
                    {
                        await _navigation.NavigateTo<VideoViewModel>();
                        break;
                    }
            }
        }

        private ObservableCollection<ShellMenuItemViewModel> GetMenuItems()
        {
            return new ObservableCollection<ShellMenuItemViewModel>
            {
                _photoMenuItem,
                _videoMenuItem,
            };
        }
    }
}
