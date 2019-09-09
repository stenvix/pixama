using System.Reactive.Disposables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Pixama.Logic.ViewModels.Photo;
using ReactiveUI;

namespace Pixama.App.Views.Photo
{
    public class PhotoGridItemViewBase : ReactiveUserControl<PhotoGridItemViewModel>
    {
    }

    public sealed partial class PhotoGridItemView : PhotoGridItemViewBase
    {
        public PhotoGridItemView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.FileName,
                        v => v.FileName.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.DateTaken,
                        v => v.FileDateTaken.Text,
                        value => value.ToShortDateString())
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.FileType,
                        v => v.FileType.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Thumbnail,
                        v => v.Thumbnail.Source, 
                        value => value as BitmapImage)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsLoading,
                        v => v.ProgressRing.Visibility)
                    .DisposeWith(disposable);
            });
        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                await ViewModel.LoadAsync();
            }
        }
    }
}
