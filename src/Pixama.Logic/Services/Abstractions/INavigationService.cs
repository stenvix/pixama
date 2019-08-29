using System.Threading.Tasks;

namespace Pixama.Logic.Services
{
    public interface INavigationService
    {
        Task<bool> NavigateTo<TViewModel>();
        Task<bool> NavigateTo<TViewModel>(object parameters);
    }
}
