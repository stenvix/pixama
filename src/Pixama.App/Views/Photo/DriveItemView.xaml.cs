using Pixama.Logic.ViewModels.Common;
using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Xaml;

namespace Pixama.App.Views.Photo
{
    public class DriveItemViewBase : ReactiveUserControl<DriveViewModel> { }
    public sealed partial class DriveItemView : DriveItemViewBase
    {
        public DriveItemView()
        {
            InitializeComponent();
            Loading += OnLoading;
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.Name,
                        v => v.DriveName.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Glyph,
                        v => v.Glyph.Glyph)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.ExpandGlyph,
                        v => v.ExpandGlyph.Glyph)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Children,
                        v => v.Children.ItemsSource)
                    .DisposeWith(disposable);

                //Visibility

                this.OneWayBind(ViewModel,
                        vm => vm.HasUnrealizedChildren,
                        v => v.ExpandButton.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsLoading,
                        v => v.IsLoading.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsExpanded,
                        v => v.Children.Visibility)
                    .DisposeWith(disposable);

                //Commands
                this.BindCommand(ViewModel,
                    vm => vm.ExpandCommand,
                    v => v.ExpandButton)
                    .DisposeWith(disposable);

            });
        }

        private async void OnLoading(FrameworkElement sender, object args)
        {
            await ViewModel.LoadAsync();
        }
    }
}
