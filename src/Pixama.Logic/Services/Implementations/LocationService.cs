using DynamicData;
using Newtonsoft.Json;
using Pixama.Logic.Enums;
using Pixama.Logic.Helpers;
using Pixama.Logic.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;

namespace Pixama.Logic.Services
{
    public class LocationService : ILocationService
    {
        private readonly string _sourceFolderListKey = "pixama-picture-source-folders";
        private readonly string _destinationFolderListKey = "pixama-picture-destination-folders";
        private readonly string _downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

        public async Task LoadSourceFoldersAsync(SourceList<SourceFolderViewModel> foldersList)
        {
            var folders = new List<SourceFolderViewModel>
            {
                //new FolderViewModel(this){Name = "Desktop", Glyph = "\uE8FC", StorageFolder = null, IsStatic = true},
                //new FolderViewModel(this){Name = "Downloads", Glyph = "\uE896", StorageFolder = await StorageFolder.GetFolderFromPathAsync(_downloadsPath), IsStatic = true},
                new SourceFolderViewModel(this){Name = "Pictures", Glyph = "\uEB9F", StorageFolder = KnownFolders.PicturesLibrary, IsStatic = true},
                new SourceFolderViewModel(this){Name = "Videos", Glyph = "\uEA69", StorageFolder = KnownFolders.VideosLibrary, IsStatic = true},
            };
            var userFolders = await GetSourceUserFolders();
            foldersList.Edit(list =>
            {
                list.Clear();
                list.AddRange(folders);
                list.AddRange(userFolders);
            });
        }

        public async Task LoadDestinationFoldersAsync(SourceList<DestinationFolderViewModel> foldersList)
        {
            var userFolders = await GetDestinationUserFolders();
            foldersList.Edit(list =>
            {
                list.Clear();
                list.AddRange(userFolders);
            });
        }

        public async Task<StorageFolder> SelectStorageFolderAsync()
        {
            var folderPicker = new FolderPicker { SuggestedStartLocation = PickerLocationId.ComputerFolder };
            folderPicker.FileTypeFilter.Add("*");
            return await folderPicker.PickSingleFolderAsync();
        }

        public bool SaveToFavorites(StorageFolder folder, LocationType locationType)
        {
            var tokens = GetTokens(locationType);
            if (tokens.ContainsKey(folder.Path)) return false;
            var token = StorageApplicationPermissions.FutureAccessList.Add(folder);
            tokens.Add(folder.Path, token);
            UpdateTokens(tokens, locationType);
            return true;
        }

        public bool RemoveFromFavoritesAsync(StorageFolder folder, LocationType locationType)
        {
            var tokens = GetTokens(locationType);
            if (!tokens.ContainsKey(folder.Path)) return false;
            tokens.Remove(folder.Path, out string token);
            StorageApplicationPermissions.FutureAccessList.Remove(token);
            UpdateTokens(tokens, locationType);
            return true;
        }

        private async Task<List<SourceFolderViewModel>> GetSourceUserFolders()
        {
            var folders = new List<SourceFolderViewModel>();
            var tokens = GetTokens(LocationType.Source);

            foreach (var token in tokens)
            {
                try
                {
                    var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token.Value);
                    folders.Add(new SourceFolderViewModel(this) { Name = storageFolder.DisplayName, Glyph = Glyphs.FolderGlyph, StorageFolder = storageFolder });
                }
                catch (Exception e)
                {
                    //Todo: Show dialog to user about missing folders
                }
            }

            return folders;
        }

        private async Task<IList<DestinationFolderViewModel>> GetDestinationUserFolders()
        {
            var folders = new List<DestinationFolderViewModel>();
            var tokens = GetTokens(LocationType.Destination);

            foreach (var token in tokens)
            {
                try
                {
                    var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token.Value);
                    folders.Add(new DestinationFolderViewModel(this) { Name = storageFolder.DisplayName, Glyph = Glyphs.FolderGlyph, StorageFolder = storageFolder });
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
                var model = new DriveViewModel(this) { Name = folder.DisplayName, Glyph = Glyphs.DriveGlyph, StorageFolder = folder };
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

        private Dictionary<string, string> GetExistedTokens(LocationType locationType)
        {
            var tokensString = (string)ApplicationData.Current.LocalSettings.Values[GetLocationKey(locationType)];
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(tokensString);
        }

        private Dictionary<string, string> GetTokens(LocationType locationType)
        {
            var exists = TokensExists(locationType);
            return exists ? GetExistedTokens(locationType) : new Dictionary<string, string>();
        }

        private void UpdateTokens(Dictionary<string, string> tokens, LocationType locationType)
        {
            var exists = TokensExists(locationType);
            var key = GetLocationKey(locationType);
            if (exists)
            {
                ApplicationData.Current.LocalSettings.Values[key] = JsonConvert.SerializeObject(tokens);
            }
            else
            {
                var str = JsonConvert.SerializeObject(tokens);
                ApplicationData.Current.LocalSettings.Values.Add(key, str);
            }
        }

        private bool TokensExists(LocationType locationType)
        {
            //ApplicationData.Current.LocalSettings.Values.Remove(_folderListKey);
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(GetLocationKey(locationType));
        }

        private string GetLocationKey(LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Source:
                    {
                        return _sourceFolderListKey;
                    }
                case LocationType.Destination:
                    {
                        return _destinationFolderListKey;
                    }
            }

            throw new InvalidOperationException("Location type is not supported");
        }

        #endregion
    }
}
