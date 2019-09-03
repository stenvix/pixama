using Pixama.Logic.ViewModels.Events;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Input;

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

        //Commands
        public ReactiveCommand<PointerRoutedEventArgs, Unit> ItemClickedCommand { get; }


        #endregion

        public BaseLocationViewModel()
        {
            MessageBus.Current.Listen<LocationChanged>().Subscribe(OnLocationChanged);
            ItemClickedCommand = ReactiveCommand.CreateFromTask<PointerRoutedEventArgs, Unit>(OnItemClicked);
        }

        private void OnLocationChanged(LocationChanged eventArgs)
        {
            IsSelected = eventArgs.Location == this;
        }

        private Task<Unit> OnItemClicked(PointerRoutedEventArgs args)
        {
            MessageBus.Current.SendMessage(new LocationChanged(this));
            return Task.FromResult(Unit.Default);
        }
    }
}
