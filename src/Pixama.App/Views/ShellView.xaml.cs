using Pixama.App.Configuration;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Shell;
using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pixama.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page, IViewFor<ShellViewModel>
    {
        #region Fields

        public ShellViewModel ViewModel { get; set; }

        #endregion

        #region Properties

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ShellViewModel)value;
        }

        #endregion

        public Shell()
        {
            InitializeComponent();
            ViewModel = ServiceLocator.Current.GetService<ShellViewModel>();
            InitializeNavigation();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.MenuItems,
                        v => v.Navigation.MenuItemsSource)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                        vm => vm.SelectedTab,
                        v => v.Navigation.SelectedItem)
                    .DisposeWith(disposable);
            });
        }

        private void InitializeNavigation()
        {
            var frameAdapter = ServiceLocator.Current.GetService<IFrameAdapter>();
            frameAdapter.SetFrame(shellFrame);
        }
    }
}
