using DynamicData;
using Pixama.Logic.Services;
using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace Pixama.Logic.ViewModels.Common
{
    public abstract class BaseDriveLocationViewModel : BaseLocationViewModel
    {
        #region Fields

        private bool _isExpanded;
        private bool _hasUnrealizedChildren;

        private readonly ReadOnlyObservableCollection<DriveLocationViewModel> _children;

        protected IDriveService DriveService;
        protected readonly SourceList<DriveLocationViewModel> ChildrenList;

        #endregion

        #region Properties

        public abstract string ExpandGlyph { get; }
        public ReadOnlyObservableCollection<DriveLocationViewModel> Children => _children;
        public bool IsExpanded { get => _isExpanded; set => this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        public bool HasUnrealizedChildren { get => _hasUnrealizedChildren; set => this.RaiseAndSetIfChanged(ref _hasUnrealizedChildren, value); }

        #endregion


        protected BaseDriveLocationViewModel(IDriveService driveService)
        {
            DriveService = driveService;
            ChildrenList = new SourceList<DriveLocationViewModel>();

            ChildrenList.Connect()
                .Bind(out _children)
                .Subscribe();
        }
    }
}
