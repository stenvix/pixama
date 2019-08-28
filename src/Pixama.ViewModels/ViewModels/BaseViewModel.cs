using ReactiveUI;
using System.Threading.Tasks;

namespace Pixama.ViewModels.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject
    {
        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
