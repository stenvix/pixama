using Pixama.Logic.ViewModels.Common;

namespace Pixama.Logic.ViewModels.Events
{
    public class SourceLocationRemoved
    {
        public SourceFolderViewModel Location { get; }

        public SourceLocationRemoved(SourceFolderViewModel location)
        {
            Location = location;
        }
    }
}
