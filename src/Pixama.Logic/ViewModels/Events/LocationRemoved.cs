using Pixama.Logic.ViewModels.Common;

namespace Pixama.Logic.ViewModels.Events
{
    public class LocationRemoved
    {
        public FolderViewModel Location { get; }

        public LocationRemoved(FolderViewModel location)
        {
            Location = location;
        }
    }
}
