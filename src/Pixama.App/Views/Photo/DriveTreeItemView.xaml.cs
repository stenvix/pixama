using Pixama.Logic.ViewModels.Common;
using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Xaml;

namespace Pixama.App.Views.Photo
{
    public class DriveTreeItemViewBase : ReactiveUserControl<DriveLocationViewModel> { }
    public sealed partial class DriveTreeItemView : DriveTreeItemViewBase
    {
        public DriveTreeItemView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Name,
                    v => v.Name.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsLoading,
                        v => v.ProgressRing.IsActive)
                    .DisposeWith(disposable);
            });
        }
    }
}
