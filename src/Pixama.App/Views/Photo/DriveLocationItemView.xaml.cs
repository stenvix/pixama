using Pixama.Logic.ViewModels.Common;
using Pixama.Logic.ViewModels.Events;
using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Xaml.Input;

namespace Pixama.App.Views.Photo
{
    public class DriveLocationItemViewBase : ReactiveUserControl<DriveLocationViewModel> { }
    public sealed partial class DriveLocationItemView : DriveLocationItemViewBase
    {
        public DriveLocationItemView()
        {
            InitializeComponent();
            ExpandGlyph.PointerPressed += ExpandGlyphOnPointerPressed;
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
                        vm => vm.ExpandGlyph,
                        v => v.ExpandGlyph.Glyph)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    vm => vm.Children,
                    v => v.Children.ItemsSource)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsSelected,
                        v => v.LocationItem.IsSelected)
                    .DisposeWith(disposable);

                //Visibility
                this.OneWayBind(ViewModel,
                        vm => vm.IsExpanded,
                        v => v.Children.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.HasUnrealizedChildren,
                        v => v.ExpandGlyph.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    vm => vm.IsLoading,
                    v => v.IsLoading.Visibility)
                    .DisposeWith(disposable);
            });
        }

        private void ExpandGlyphOnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.IsExpanded = !ViewModel.IsExpanded;
        }
    }
}
