using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Pixama.Logic.Services
{
    public class NavigationService : INavigationService
    {
        #region Fields

        private static readonly ConcurrentDictionary<Type, Type> ViewModelsMap = new ConcurrentDictionary<Type, Type>();
        private readonly IFrameAdapter _frameAdapter;

        #endregion

        #region Constructors

        public NavigationService(IFrameAdapter frameAdapter)
        {
            _frameAdapter = frameAdapter;
        }

        #endregion

        #region Methods

        public static void Register<TViewModel, TView>()
        {
            if (!ViewModelsMap.TryAdd(typeof(TViewModel), typeof(TView)))
            {
                throw new InvalidOperationException($"ViewModel already registered '{typeof(TViewModel).FullName}'");
            }
        }

        #endregion

        #region Interface methods

        public Task<bool> NavigateTo<TViewModel>()
        {
            return NavigateTo<TViewModel>(null);
        }

        public async Task<bool> NavigateTo<TViewModel>(object parameters)
        {
            if (!_frameAdapter.CanNavigate) return false;
            var navigated = false;
            await Window.Current.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 navigated = _frameAdapter.Navigate(GetView(typeof(TViewModel)), parameters);
             });
            return navigated;
        }

        #endregion

        #region Helpers

        private Type GetView(Type viewModelType)
        {
            if (ViewModelsMap.TryGetValue(viewModelType, out var viewType)) return viewType;
            throw new InvalidOperationException($"View not registered for ViewModel '{viewModelType.FullName}'");
        }

        #endregion
    }
}
