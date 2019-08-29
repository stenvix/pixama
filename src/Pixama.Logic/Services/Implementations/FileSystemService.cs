using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using DynamicData;
using Pixama.Logic.ViewModels.Common;

namespace Pixama.Logic.Services
{
    public class FileSystemService : IFileSystemService
    {
        public Task GetFolders(SourceList<StorageLocationViewModel> foldersList)
        {
            foldersList.Edit(list =>
            {
                list.Clear();
                //list.Add(new StorageLocationViewModel("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "\uE8FC"));
                //list.Add(new StorageLocationViewModel("Downloads", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads", "\uE896"));
                //list.Add(new StorageLocationViewModel("Documents", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "\uE8A5"));
                //list.Add(new StorageLocationViewModel("Pictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "\uE8A5"));
                //list.Add(new StorageLocationViewModel("Videos", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "\uE8A5"));
            });

            return Task.CompletedTask;
        }

        public async Task GetDrives(SourceList<StorageLocationViewModel> drivesList)
        {
            var drives = DriveInfo.GetDrives();
            var removable = await KnownFolders.RemovableDevices.GetFoldersAsync();
            var removableDrives = removable.Select(i => i.Name).ToList();
            var driveModels = new List<StorageLocationViewModel>();

            foreach (var drive in drives)
            {
                var isRemovable = removableDrives.Contains(drive.RootDirectory.Root.Name);
                var driveType = isRemovable ? DriveType.Removable : drive.DriveType;
                var name = GetName(drive.Name, driveType);
                var glyph = GetGlyph(driveType);

                //var model = new StorageLocationViewModel(name, drive.RootDirectory.Root.FullName, glyph);
                //driveModels.Add(model);
            }

            drivesList.Edit(models =>
            {
                models.Clear();
                models.AddRange(driveModels);
            });
        }

        private string GetGlyph(DriveType driveType)
        {
            switch (driveType)
            {
                case DriveType.Fixed:
                    {
                        return "\uEDA2";
                    }
                case DriveType.Removable:
                    {
                        return "\uE88E";
                    }
                case DriveType.Network:
                    {
                        return "\uE753";
                    }
                default:
                    {
                        return "\uEA6C";
                    }
            }
        }

        private string GetName(string name, DriveType driveType)
        {
            switch (driveType)
            {
                case DriveType.Fixed:
                    {
                        return $"Local Disk ({name})";
                    }
                case DriveType.Removable:
                    {
                        return $"Removable Drive ({name})";
                    }
                case DriveType.CDRom:
                    {
                        return $"CD Disk ({name})";
                    }
                case DriveType.Network:
                    {
                        return $"Network Disk ({name})";
                    }
                default:
                    {
                        return $"Disk ({name})";
                    }
            }
        }
    }
}
