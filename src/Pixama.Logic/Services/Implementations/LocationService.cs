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
        private readonly string _folderListKey = "pixama-folders";
        private readonly string _downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

        public async Task LoadFoldersAsync(SourceList<FolderViewModel> foldersList)
        {
            var folders = new List<FolderViewModel>
            {
                //new FolderViewModel(this){Name = "Desktop", Glyph = "\uE8FC", StorageFolder = null, IsStatic = true},
                //new FolderViewModel(this){Name = "Downloads", Glyph = "\uE896", StorageFolder = await StorageFolder.GetFolderFromPathAsync(_downloadsPath), IsStatic = true},
                new FolderViewModel(this){Name = "Pictures", Glyph = "\uEB9F", StorageFolder = KnownFolders.PicturesLibrary, IsStatic = true},
                new FolderViewModel(this){Name = "Videos", Glyph = "\uEA69", StorageFolder = KnownFolders.VideosLibrary, IsStatic = true},
            };
            var userFolders = await GetUserFolders();
            foldersList.Edit(list =>
            {
                list.Clear();
                list.AddRange(folders);
                list.AddRange(userFolders);
            });
        }

        public Task LoadFolderAsync(StorageFolder storageFolder, string token, SourceList<FolderViewModel> foldersList)
        {
            var model = new FolderViewModel(this, token) { Name = storageFolder.DisplayName, Glyph = _folderGlyph, StorageFolder = storageFolder };
            foldersList.Add(model);
            return Task.CompletedTask;
        }

        public async Task<StorageFolder> SelectStorageFolderAsync()
        {
            var folderPicker = new FolderPicker { SuggestedStartLocation = PickerLocationId.ComputerFolder };
            folderPicker.FileTypeFilter.Add("*");
            return await folderPicker.PickSingleFolderAsync();
        }

        public bool SaveToFavorites(StorageFolder folder, out string token)
        {
            token = StorageApplicationPermissions.FutureAccessList.Add(folder);
            var tokens = GetTokenList();
            if (tokens.Contains(token)) return false;
            SetTokenList(tokens);
            return true;
        }

        public Task RemoveFromFavoritesAsync(string token)
        {
            StorageApplicationPermissions.FutureAccessList.Remove(token);
            var tokens = GetTokenList();
            tokens.Remove(token);
            SetTokenList(tokens);
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
                try
                {
                    var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                    folders.Add(new FolderViewModel(this, token) { Name = storageFolder.DisplayName, Glyph = _folderGlyph, StorageFolder = storageFolder });
                }
                catch (Exception e)
                {
                    //Todo: Show dialog to user about missing folders
                }
            }

            return folders;
        }

        public async Task LoadDrivesAsync(SourceList<DriveViewModel> drivesList)
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

        public async Task GetChildrenFoldersAsync(StorageFolder sourceFolder, SourceList<LocationViewModel> childrenFoldersList)
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

        private List<string> GetTokenList()
        {
            var exists = TokenListExists();
            return exists ? GetExistedTokens() : new List<string>();
        }

        private void SetTokenList(List<string> tokens)
        {
            var exists = TokenListExists();
            if (exists)
            {
                ApplicationData.Current.LocalSettings.Values[_folderListKey] = JsonConvert.SerializeObject(tokens.Distinct());
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values.Add(_folderListKey, JsonConvert.SerializeObject(tokens));
            }
        }

        private bool TokenListExists()
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(_folderListKey);
        }
    }
}
