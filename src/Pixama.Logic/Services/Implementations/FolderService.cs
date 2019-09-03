using DynamicData;
using Newtonsoft.Json;
using Pixama.Logic.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace Pixama.Logic.Services
{
    public class FolderService : IFolderService
    {
        private static string _folderListKey = "pixama-folders";
        public async Task GetFolders(SourceList<BaseLocationViewModel> foldersList)
        {
            var folders = new List<BaseLocationViewModel>
            {
                new BaseLocationViewModel{Name = "Desktop", Glyph = "\uE8FC", StorageFolder = null},
                new BaseLocationViewModel{Name = "Downloads", Glyph = "\uE896", StorageFolder = null},
                //new BaseLocationViewModel{Name = "Documents", Glyph = "\uE8A5", StorageFolder = KnownFolders.DocumentsLibrary},
                new BaseLocationViewModel{Name = "Pictures", Glyph = "\uEB9F", StorageFolder = KnownFolders.PicturesLibrary},
                new BaseLocationViewModel{Name = "Videos", Glyph = "\uEA69", StorageFolder = KnownFolders.VideosLibrary},

                //new BaseLocationViewModel("","\", null,
                //    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads"),
                //new BaseLocationViewModel("", "\uE8A5", null,
                //    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                //new BaseLocationViewModel("Pictures", "\uEB9F", null,
                //    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
                //new BaseLocationViewModel("Videos", "\uEA69", null,
                //    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos))
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

        private async Task<List<BaseLocationViewModel>> GetUserFolders()
        {
            var folders = new List<BaseLocationViewModel>();

            var hasFolders = ApplicationData.Current.LocalSettings.Values.ContainsKey(_folderListKey);
            if (!hasFolders) return folders;

            var tokens = GetExistedTokens();

            foreach (var token in tokens)
            {
                var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                folders.Add(new BaseLocationViewModel { Name = storageFolder.Name, Glyph = "\uF12B", StorageFolder = storageFolder });
            }

            return folders;
        }
    }
}
