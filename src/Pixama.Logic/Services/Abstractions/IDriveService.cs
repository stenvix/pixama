using DynamicData;
using Pixama.Logic.ViewModels.Common;
using System.Threading.Tasks;

namespace Pixama.Logic.Services
{
    public interface IDriveService
    {
        Task GetDrives(SourceList<StorageLocationViewModel> drivesList);
    }
}
