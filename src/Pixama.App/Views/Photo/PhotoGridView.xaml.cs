using Pixama.App.Configuration;
using Pixama.Logic.ViewModels.Events;
using Pixama.Logic.ViewModels.Photo;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using Windows.UI.Xaml.Data;

namespace Pixama.App.Views.Photo
{
    public class PhotoGridViewBase : ReactiveUserControl<PhotoGridViewModel> { }
    public sealed partial class PhotoGridView : PhotoGridViewBase
    {
        public PhotoGridView()
        {
            InitializeComponent();
            ViewModel = ServiceLocator.Current.GetService<PhotoGridViewModel>();
            MessageBus.Current.Listen<SelectAllPhotos>().Subscribe(OnSelectAll);
            MessageBus.Current.Listen<DeselectAllPhotos>().Subscribe(OnDeselectAll);

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.FreshLoading,
                        v => v.PhotoGridPlaceholder.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    vm => vm.SourceSelected,
                    v => v.PhotoGrid.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.SourceSelected,
                        v => v.PhotoBar.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsLoading,
                        v => v.IsLoading.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Photos,
                        v => v.PhotoGrid.ItemsSource)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                        vm => vm.SelectAllCommand,
                        v => v.SelectAllButton)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                        vm => vm.DeselectAllCommand,
                        v => v.DeselectAllButton)
                    .DisposeWith(disposable);
            });
        }

        private void OnSelectAll(SelectAllPhotos args)
        {
            PhotoGrid.SelectAll();
        }

        private void OnDeselectAll(DeselectAllPhotos args)
        {
            PhotoGrid.DeselectRange(new ItemIndexRange(0, (uint)ViewModel.Photos.Count));
        }
    }
}
