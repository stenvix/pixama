using DynamicData;
using Pixama.Logic.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Pixama.Logic.Services
{
    public class DriveService : IDriveService
    {
        private readonly string _driveGlyph = "\uE88E";
        public async Task GetDrives(SourceList<DriveViewModel> drivesList)
        {
            var removable = await KnownFolders.RemovableDevices.GetFoldersAsync();
            var drives = new List<DriveViewModel>();
            foreach (var folder in removable)
            {
                var model = new DriveViewModel(this) { Name = folder.Name, Glyph = _driveGlyph, StorageFolder = folder };
                drives.Add(model);
            }

            drivesList.Edit(list =>
            {
                list.Clear();
                list.AddRange(drives);
            });
        }

        public async Task GetChildrenFoldersAsync(IStorageFolder sourceFolder, SourceList<DriveLocationViewModel> childrenFoldersList)
        {
            var storageFolders = await sourceFolder.GetFoldersAsync();

            var childrenFolders = new List<DriveLocationViewModel>();
            foreach (var storageFolder in storageFolders)
            {
                var model = new DriveLocationViewModel(this)
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

        private async Task<bool> HasChildFoldersAsync(StorageFolder sourceFolder)
        {
            var name = sourceFolder.Name;
            var folders = await sourceFolder.GetFoldersAsync();

            //var queryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery)
            //{
            //    FolderDepth = FolderDepth.Shallow,
            //    IndexerOption = IndexerOption.UseIndexerWhenAvailable,
            //};
            //var query = sourceFolder.CreateFolderQueryWithOptions(queryOptions);
            //var items = await query.GetItemCountAsync();
            var count = folders.Count;
            return count != 0;
        }
    }
}
