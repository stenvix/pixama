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

        public Task LoadFolderAsync(StorageFolder storageFolder, SourceList<FolderViewModel> foldersList)
        {
            var model = new FolderViewModel(this) { Name = storageFolder.DisplayName, Glyph = _folderGlyph, StorageFolder = storageFolder };
            foldersList.Add(model);
            return Task.CompletedTask;
        }

        public async Task<StorageFolder> SelectStorageFolderAsync()
        {
            var folderPicker = new FolderPicker { SuggestedStartLocation = PickerLocationId.ComputerFolder };
            folderPicker.FileTypeFilter.Add("*");
            return await folderPicker.PickSingleFolderAsync();
        }

        public bool SaveToFavorites(StorageFolder folder)
        {
            var tokens = GetTokens();
            if (tokens.ContainsKey(folder.Path)) return false;
            var token = StorageApplicationPermissions.FutureAccessList.Add(folder);
            tokens.Add(folder.Path, token);
            UpdateTokens(tokens);
            return true;
        }

        public bool RemoveFromFavoritesAsync(StorageFolder folder)
        {
            var tokens = GetTokens();
            if (!tokens.ContainsKey(folder.Path)) return false;
            tokens.Remove(folder.Path, out string token);
            StorageApplicationPermissions.FutureAccessList.Remove(token);
            UpdateTokens(tokens);
            return true;
        }

        private async Task<List<FolderViewModel>> GetUserFolders()
        {
            var folders = new List<FolderViewModel>();
            var tokens = GetTokens();

            foreach (var token in tokens)
            {
                try
                {
                    var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token.Value);
                    folders.Add(new FolderViewModel(this) { Name = storageFolder.DisplayName, Glyph = _folderGlyph, StorageFolder = storageFolder });
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

        #region Tokens

        private Dictionary<string, string> GetExistedTokens()
        {
            var tokensString = (string)ApplicationData.Current.LocalSettings.Values[_folderListKey];
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(tokensString);
        }

        private Dictionary<string, string> GetTokens()
        {
            var exists = TokensExists();
            return exists ? GetExistedTokens() : new Dictionary<string, string>();
        }

        private void UpdateTokens(Dictionary<string, string> tokens)
        {
            var exists = TokensExists();
            if (exists)
            {
                ApplicationData.Current.LocalSettings.Values[_folderListKey] = JsonConvert.SerializeObject(tokens);
            }
            else
            {
                var str = JsonConvert.SerializeObject(tokens);
                ApplicationData.Current.LocalSettings.Values.Add(_folderListKey, str);
            }
        }

        private bool TokensExists()
        {
            //ApplicationData.Current.LocalSettings.Values.Remove(_folderListKey);
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(_folderListKey);
        }

        #endregion
    }
}
