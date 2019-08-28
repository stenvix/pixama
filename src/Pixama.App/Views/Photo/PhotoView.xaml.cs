using System.Reactive.Disposables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Pixama.App.Services;
using Pixama.ViewModels.Photo;
using ReactiveUI;

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
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.IsLoading,
                        v => v.ProgressRing.IsActive)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsReady,
                        v => v.Sidebar.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Folders,
                        v => v.Folders.ItemsSource)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Drives,
                        v => v.Drives.ItemsSource)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    vm => vm.SelectedDrive,
                    v => v.Drives.SelectedItem);

                this.Bind(ViewModel,
                    vm => vm.SelectedFolder,
                    v => v.Folders.SelectedItem);
            });
        }

        private async void OnLoading(FrameworkElement sender, object args)
        {
            await ViewModel.LoadAsync();
        }
    }
}
