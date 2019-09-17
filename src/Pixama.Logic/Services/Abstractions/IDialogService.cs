using System.Threading.Tasks;

namespace Pixama.Logic.Services
{
    public interface IDialogService
    {
        Task<bool> ManyFilesDialog();
    }
}
