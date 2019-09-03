using Pixama.Logic.ViewModels.Common;

namespace Pixama.Logic.ViewModels.Events
{
    public class LocationChanged
    {
        public BaseLocationViewModel Location { get; }

        public LocationChanged(BaseLocationViewModel location)
        {
            Location = location;
        }
    }
}
