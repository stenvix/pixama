using DynamicData;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Events;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Windows.Storage;
using Windows.UI.Xaml.Input;

namespace Pixama.Logic.ViewModels.Common
{
    public abstract class BaseLocationViewModel : BaseViewModel
    {
        #region Fields

        private string _name;
        private StorageFolder _storageFolder;
        private string _glyph;
        private bool _isSelected;
        private bool _isExpanded;
        private bool _hasUnrealizedChildren;
        private readonly ReadOnlyObservableCollection<LocationViewModel> _children;

        protected ILocationService LocationService;
        protected readonly SourceList<LocationViewModel> ChildrenList;

        #endregion

        #region Properties

        public abstract string ExpandGlyph { get; }
        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public string Glyph { get => _glyph; set => this.RaiseAndSetIfChanged(ref _glyph, value); }
        public StorageFolder StorageFolder { get => _storageFolder; set => this.RaiseAndSetIfChanged(ref _storageFolder, value); }
        public bool IsSelected { get => _isSelected; set => this.RaiseAndSetIfChanged(ref _isSelected, value); }
        public bool IsExpanded { get => _isExpanded; set => this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        public bool HasUnrealizedChildren { get => _hasUnrealizedChildren; set => this.RaiseAndSetIfChanged(ref _hasUnrealizedChildren, value); }

        public ReadOnlyObservableCollection<LocationViewModel> Children => _children;

        //Commands
        public ReactiveCommand<TappedRoutedEventArgs, Unit> ExpandCommand { get; }
        public ReactiveCommand<TappedRoutedEventArgs, Unit> ItemClickCommand { get; }

        #endregion

        protected BaseLocationViewModel(ILocationService locationService)
        {
            LocationService = locationService;
            ChildrenList = new SourceList<LocationViewModel>();

            ChildrenList.Connect()
                .Bind(out _children)
                .Subscribe();

            MessageBus.Current.Listen<LocationChanged>().Subscribe(OnLocationChanged);

            ItemClickCommand = ReactiveCommand.Create<TappedRoutedEventArgs>(OnItemClick);
            ExpandCommand = ReactiveCommand.Create<TappedRoutedEventArgs>(ToggleChildrenVisibility);
        }

        private void ToggleChildrenVisibility(TappedRoutedEventArgs args)
        {
            IsExpanded = !IsExpanded;
            this.RaisePropertyChanged(nameof(ExpandGlyph));
            if (args != null) args.Handled = true;
        }

        private void OnLocationChanged(LocationChanged eventArgs)
        {
            IsSelected = eventArgs.Location == this;
        }

        private void OnItemClick(TappedRoutedEventArgs args)
        {
            MessageBus.Current.SendMessage(new LocationChanged(this));
        }
    }
}
