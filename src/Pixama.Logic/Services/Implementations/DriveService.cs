using DynamicData;
using Pixama.Logic.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixama.Logic.Services
{
    public class DriveService : IDriveService
    {
        public async Task GetDrives(SourceList<StorageLocationViewModel> drivesList)
        {
            var removable = await KnownFolders.RemovableDevices.GetFoldersAsync();
            var drives = new List<StorageLocationViewModel>();
            foreach (var folder in removable)
            {
                var name = $"Removable Drive ({folder.Name})";
                var glyph = "\uE88E";
                var model = new StorageLocationViewModel(name, glyph, folder);
                drives.Add(model);
            }

            drivesList.Edit(list =>
            {
                list.Clear();
                list.AddRange(drives);
            });
        }
    }
}
