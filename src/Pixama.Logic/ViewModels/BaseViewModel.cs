using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace Pixama.Logic.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject
    {
        #region Fields

        private bool _isLoading;
        private readonly ObservableAsPropertyHelper<bool> _isReady;

        #endregion

        #region Properties

        public bool IsReady => _isReady.Value;

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        #endregion

        protected BaseViewModel()
        {
            IsLoading = false;
            _isReady = ObservableAsPropertyHelper<bool>.Default();
            this.WhenAnyValue(i => i.IsLoading)
                .Select(i => !i)
                .ToProperty(this, i => i.IsReady, out _isReady);
        }

        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
