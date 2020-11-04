using System.Linq;
using System.Reactive.Disposables;
using Windows.UI.Xaml.Controls;
using Pixama.App.Configuration;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels;
using ReactiveUI;

namespace Pixama.App.Views
{
    public sealed partial class LayoutView : Page, IViewFor<LayoutViewModel>
    {
        public LayoutView()
        {
            this.InitializeComponent();
            this.ViewModel = ServiceLocator.Current.GetService<LayoutViewModel>();
            this.InitializeNavigation();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(this.ViewModel,
                        vm => vm.LayoutMenuItems,
                        v => v.Navigation.MenuItemsSource)
                    .DisposeWith(disposable);

                this.Bind(this.ViewModel,
                        vm => vm.SelectedMenuItem,
                        v => v.Navigation.SelectedItem)
                    .DisposeWith(disposable);

                this.Navigation.SelectedItem = this.ViewModel.LayoutMenuItems.First();
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (LayoutViewModel)value;
        }

        public LayoutViewModel ViewModel { get; set; }

        private void InitializeNavigation()
        {
            var frameAdapter = ServiceLocator.Current.GetService<IFrameAdapter>();
            frameAdapter.SetFrame(MainFrame);
        }
    }
}
