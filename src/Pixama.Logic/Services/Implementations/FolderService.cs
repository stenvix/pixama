using DynamicData;
using Pixama.Logic.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Newtonsoft.Json;

namespace Pixama.Logic.Services
{
    public class FolderService : IFolderService
    {
        private static string _folderListKey = "pixama-folders";
        public async Task GetFolders(SourceList<StorageLocationViewModel> foldersList)
        {
            var folders = new List<StorageLocationViewModel>
            {
                new StorageLocationViewModel("Desktop", "\uE8FC", null,
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)),
                new StorageLocationViewModel("Downloads","\uE896", null,
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads"),
                new StorageLocationViewModel("Documents", "\uE8A5", null,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                new StorageLocationViewModel("Pictures", "\uEB9F", null,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
                new StorageLocationViewModel("Videos", "\uEA69", null,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos))
            };
            var userFolders = await GetUserFolders();
            foldersList.Edit(list =>
            {
                list.Clear();
                list.AddRange(folders);
                list.AddRange(userFolders);
            });
        }

        public async Task<IStorageFolder> SelectStorageFolderAsync()
        {
            var folderPicker = new FolderPicker { SuggestedStartLocation = PickerLocationId.ComputerFolder };
            folderPicker.FileTypeFilter.Add("*");
            return await folderPicker.PickSingleFolderAsync();
        }

        public Task SaveToFavoritesAsync(IStorageFolder folder)
        {
            var token = StorageApplicationPermissions.FutureAccessList.Add(folder);
            var exists = ApplicationData.Current.LocalSettings.Values.ContainsKey(_folderListKey);
            if (exists)
            {
                var tokens = GetExistedTokens();
                tokens.Add(token);
                ApplicationData.Current.LocalSettings.Values[_folderListKey] = JsonConvert.SerializeObject(tokens.Distinct());
            }
            else
            {
                var tokens = new List<string> { token };
                ApplicationData.Current.LocalSettings.Values.Add(_folderListKey, JsonConvert.SerializeObject(tokens));
            }

            return Task.CompletedTask;
        }

        private List<string> GetExistedTokens()
        {
            var tokensString = (string)ApplicationData.Current.LocalSettings.Values[_folderListKey];
            return JsonConvert.DeserializeObject<List<string>>(tokensString);
        }

        private async Task<List<StorageLocationViewModel>> GetUserFolders()
        {
            var folders = new List<StorageLocationViewModel>();

            var hasFolders = ApplicationData.Current.LocalSettings.Values.ContainsKey(_folderListKey);
            if (!hasFolders) return folders;

            var tokens = GetExistedTokens();

            foreach (var token in tokens)
            {
                var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                folders.Add(new StorageLocationViewModel(storageFolder.Name, "\uF12B", storageFolder));
            }

            return folders;
        }
    }
}
