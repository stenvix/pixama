using System.Threading.Tasks;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Pages;

namespace Pixama.Logic.ViewModels.Shell
{
    public class ShellViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;

        public ShellViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        public async Task SetMainPage()
        {
            await _navigation.NavigateTo<SourcePageViewModel>();
        }
    }
}
