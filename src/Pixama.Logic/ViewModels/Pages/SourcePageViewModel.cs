using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Common;
using ReactiveUI;

namespace Pixama.Logic.ViewModels.Pages
{
    public class SourcePageViewModel : BaseViewModel
    {
        private readonly ILocationService locationService;

        private SourceList<SourceFolderViewModel> sourceFoldersList;

        private ReadOnlyObservableCollection<SourceFolderViewModel> sourceFolders;

        public ReadOnlyObservableCollection<SourceFolderViewModel> SourceFolders => this.sourceFolders;

        public SourcePageViewModel(ILocationService locationService)
        {
            this.locationService = locationService;
            this.sourceFoldersList = new SourceList<SourceFolderViewModel>();

            this.sourceFoldersList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out sourceFolders)
                .Subscribe();
        }

        public async Task LoadFoldersAsync()
        {
            await this.locationService.LoadSourceFoldersAsync(sourceFoldersList);
        }
    }
}
