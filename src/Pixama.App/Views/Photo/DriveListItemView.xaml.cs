using Pixama.Logic.ViewModels.Common;
using ReactiveUI;
using System.Reactive.Disposables;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Pixama.App.Views.Photo
{
    public class DriveListItemViewBase : ReactiveUserControl<DriveViewModel> { }
    public sealed partial class DriveListItemView : DriveListItemViewBase
    {
        public DriveListItemView()
        {
            InitializeComponent();
            Loading += OnLoading;
            if (DesignMode.DesignModeEnabled)
            {
                ViewModel = new DriveViewModel(null) { Name = "G:\\" };
            }
            Drive.ItemClick += OnClick;
            this.WhenActivated(disposable =>
                {
                    this.OneWayBind(ViewModel,
                        vm => vm.Name,
                        v => v.Name.Text)
                        .DisposeWith(disposable);

                    this.OneWayBind(ViewModel,
                            vm => vm.Glyph,
                            v => v.Glyph.Glyph)
                        .DisposeWith(disposable);

                    this.OneWayBind(ViewModel,
                            vm => vm.ShowTree,
                            v => v.Tree.Visibility)
                        .DisposeWith(disposable);

                    this.OneWayBind(ViewModel,
                            vm => vm.Children,
                            v => v.Tree.ItemsSource)
                        .DisposeWith(disposable);
                });

            Tree.ItemInvoked += TreeOnItemInvoked;
            Tree.Expanding += TreeOnExpanding;
        }

        private async void TreeOnExpanding(TreeView sender, TreeViewExpandingEventArgs args)
        {
            if (!(args.Item is DriveLocationViewModel viewModel)) return;
            await viewModel.LoadAsync();
        }

        private void TreeOnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {

        }

        private async void OnLoading(FrameworkElement sender, object args)
        {
            await ViewModel.LoadAsync();
        }

        private void OnClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.ShowTree = !ViewModel.ShowTree;
        }
    }
}
