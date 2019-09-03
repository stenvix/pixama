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
using Windows.Storage.Search;

namespace Pixama.Logic.Services
{
    public class LocationService : ILocationService
    {
        private readonly string _driveGlyph = "\uE88E";
        private readonly string _folderGlyph = "\uF12B";
        private static string _folderListKey = "pixama-folders";

        public async Task GetFolders(SourceList<FolderViewModel> foldersList)
        {
            var folders = new List<FolderViewModel>
            {
                new FolderViewModel(this){Name = "Desktop", Glyph = "\uE8FC", StorageFolder = null, IsStatic = true},
                new FolderViewModel(this){Name = "Downloads", Glyph = "\uE896", StorageFolder = null, IsStatic = true},
                //new BaseLocationViewModel{Name = "Documents", Glyph = "\uE8A5", StorageFolder = KnownFolders.DocumentsLibrary},
                new FolderViewModel(this){Name = "Pictures", Glyph = "\uEB9F", StorageFolder = KnownFolders.PicturesLibrary, IsStatic = true},
                new FolderViewModel(this){Name = "Videos", Glyph = "\uEA69", StorageFolder = KnownFolders.VideosLibrary, IsStatic = true},

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

        private async Task<List<FolderViewModel>> GetUserFolders()
        {
            var folders = new List<FolderViewModel>();

            var hasFolders = ApplicationData.Current.LocalSettings.Values.ContainsKey(_folderListKey);
            if (!hasFolders) return folders;

            var tokens = GetExistedTokens();

            foreach (var token in tokens)
            {
                var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                folders.Add(new FolderViewModel(this) { Name = storageFolder.Name, Glyph = _folderGlyph, StorageFolder = storageFolder });
            }

            return folders;
        }

        public async Task GetDrives(SourceList<DriveViewModel> drivesList)
        {
            var removable = await KnownFolders.RemovableDevices.GetFoldersAsync();
            var drives = new List<DriveViewModel>();
            foreach (var folder in removable)
            {
                var model = new DriveViewModel(this) { Name = folder.DisplayName, Glyph = _driveGlyph, StorageFolder = folder };
                drives.Add(model);
            }

            drivesList.Edit(list =>
            {
                list.Clear();
                list.AddRange(drives);
            });
        }

        public async Task GetChildrenFoldersAsync(IStorageFolder sourceFolder, SourceList<LocationViewModel> childrenFoldersList)
        {
            if (sourceFolder == null) return;
            var storageFolders = await sourceFolder.GetFoldersAsync();

            var childrenFolders = new List<LocationViewModel>();
            foreach (var storageFolder in storageFolders)
            {
                var model = new LocationViewModel(this)
                {
                    Name = storageFolder.Name,
                    StorageFolder = storageFolder,
                    HasUnrealizedChildren = await HasChildFoldersAsync(storageFolder)
                };
                childrenFolders.Add(model);
            }

            childrenFoldersList.Edit(list =>
            {
                list.Clear();
                list.AddRange(childrenFolders);
            });
        }

        public async Task<bool> HasChildFoldersAsync(StorageFolder sourceFolder)
        {
            var queryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery)
            {
                FolderDepth = FolderDepth.Shallow,
                IndexerOption = IndexerOption.UseIndexerWhenAvailable,
            };
            var query = sourceFolder.CreateFolderQueryWithOptions(queryOptions);
            var count = await query.GetItemCountAsync();
            return count != 0;
        }
    }
}
