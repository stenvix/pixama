using System.Threading.Tasks;
using Pixama.Logic.Enums;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Events;
using ReactiveUI;

namespace Pixama.Logic.ViewModels.Common
{
    public class DestinationFolderViewModel : SourceFolderViewModel
    {

        public DestinationFolderViewModel(ILocationService locationService) : base(locationService)
        {
        }

        protected override void OnFavoriteClick()
        {
            if (!LocationService.RemoveFromFavoritesAsync(StorageFolder, LocationType.Destination)) return; //Todo: show warning message
            MessageBus.Current.SendMessage(new DestinationLocationRemoved { Location = this });
        }

        public override Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
