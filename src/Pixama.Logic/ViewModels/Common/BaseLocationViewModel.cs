using System;
using ReactiveUI;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Pixama.Logic.ViewModels.Events;

namespace Pixama.Logic.ViewModels.Common
{
    public class BaseLocationViewModel : BaseViewModel
    {
        #region Fields

        private string _name;
        private StorageFolder _storageFolder;
        private string _glyph;
        private bool _isSelected;

        #endregion

        #region Properties

        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public string Glyph { get => _glyph; set => this.RaiseAndSetIfChanged(ref _glyph, value); }
        public StorageFolder StorageFolder { get => _storageFolder; set => this.RaiseAndSetIfChanged(ref _storageFolder, value); }
        public bool IsSelected { get => _isSelected; set => this.RaiseAndSetIfChanged(ref _isSelected, value); }

        #endregion

        public BaseLocationViewModel()
        {
            MessageBus.Current.Listen<ClearSelectionEvent>().Subscribe(ClearSelection);
        }

        private async void ClearSelection(ClearSelectionEvent eventArgs)
        {
            //await Window.Current.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //    () => { IsSelected = false; });
        }
    }
}
