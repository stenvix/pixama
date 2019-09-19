using Pixama.App.Configuration;
using Pixama.Logic.ViewModels.Photo;
using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Pixama.App.Views.Photo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Photo : Page, IViewFor<PhotoViewModel>
    {
        #region Fields

        public PhotoViewModel ViewModel { get; set; }

        #endregion

        #region Properties

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (PhotoViewModel)value;
        }

        #endregion

        public Photo()
        {
            ViewModel = ServiceLocator.Current.GetService<PhotoViewModel>();
            Loading += OnLoading;
            Unloaded += OnUnloaded;
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.IsLoading,
                        v => v.ProgressRing.IsActive)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsReady,
                        v => v.LeftSidebar.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsReady,
                        v => v.PhotoGrid.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsReady,
                        v => v.RightSidebar.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.SourceFolders,
                        v => v.Folders.ItemsSource)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.DestinationFolders,
                        v => v.DestinationFolders.ItemsSource)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    vm => vm.HasRemovableDrive,
                    v => v.Drives.Visibility);

                this.OneWayBind(ViewModel,
                    vm => vm.HasRemovableDrive,
                    v => v.DrivesLabel.Visibility);

                this.OneWayBind(ViewModel,
                        vm => vm.Drives,
                        v => v.Drives.ItemsSource)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    vm => vm.AddSourceFolderCommand,
                    v => v.AddSourceFolderInteraction)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                        vm => vm.AddDestinationFolderCommand,
                        v => v.AddDestinationFolderInteraction)
                    .DisposeWith(disposable);
            });
        }

        private async void OnLoading(FrameworkElement sender, object args)
        {
            await ViewModel.LoadAsync();
            ViewModel.StartDevicesTracking();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.StopDevicesTracking();
        }
    }
}
