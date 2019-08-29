using System.Threading.Tasks;
using DynamicData;
using Pixama.Logic.ViewModels.Common;

namespace Pixama.Logic.Services
{
    public interface IFileSystemService
    {
        Task GetFolders(SourceList<StorageLocationViewModel> foldersList);
        Task GetDrives(SourceList<StorageLocationViewModel> drivesList);
    }
}
