using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Pixama.Logic.Services
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ManyFilesDialog()
        {
            var dialog = new ContentDialog
            {
                Title = "Do you want to continue?",
                Content = "The folder contains more than 200 files, it may take some time to download, would you like to continue?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
