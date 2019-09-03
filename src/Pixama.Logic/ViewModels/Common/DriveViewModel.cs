using System.Collections.ObjectModel;
using Pixama.Logic.Services;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace Pixama.Logic.ViewModels.Common
{
    public class DriveViewModel : BaseDriveLocationViewModel
    {
        public override string ExpandGlyph => GetExpandGlyph();

        #region Commands

        public ReactiveCommand<Unit, Unit> ExpandCommand { get; }
        public ObservableCollection<DriveViewModel> DriveSources { get; }

        #endregion

        public DriveViewModel(IDriveService driveService) : base(driveService)
        {
            ExpandCommand = ReactiveCommand.Create(ToggleChildrenVisibility);
            DriveSources = new ObservableCollection<DriveViewModel> { this };
        }

        private void ToggleChildrenVisibility()
        {
            IsExpanded = !IsExpanded;
            this.RaisePropertyChanged(nameof(ExpandGlyph));
        }

        public override async Task LoadAsync()
        {
            IsLoading = true;
            await DriveService.GetChildrenFoldersAsync(StorageFolder, ChildrenList);
            HasUnrealizedChildren = ChildrenList.Count != 0;
            IsLoading = false;
        }

        private string GetExpandGlyph()
        {
            if (IsExpanded) return "\uE70E";
            return "\uE70D";
        }
    }
}
