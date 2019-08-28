using DynamicData;
using Pixama.ViewModels.Common;
using Pixama.ViewModels.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixama.App.Services
{
    public class FileSystemService : IFileSystemService
    {
        public Task GetFolders(SourceList<FolderViewModel> foldersList)
        {
            foldersList.Edit(list =>
            {
                list.Clear();
                list.Add(new FolderViewModel("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "\uE8FC"));
                list.Add(new FolderViewModel("Downloads", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads", "\uE896"));
                list.Add(new FolderViewModel("Documents", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "\uE8A5"));
                list.Add(new FolderViewModel("Pictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "\uE8A5"));
                list.Add(new FolderViewModel("Videos", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "\uE8A5"));
            });

            return Task.CompletedTask;
        }

        public async Task GetDrives(SourceList<DriveViewModel> drivesList)
        {
            var drives = DriveInfo.GetDrives();
            var removable = await KnownFolders.RemovableDevices.GetFoldersAsync();
            var removableDrives = removable.Select(i => i.Name).ToList();
            var driveModels = new List<DriveViewModel>();

            foreach (var drive in drives)
            {
                var isRemovable = removableDrives.Contains(drive.RootDirectory.Root.Name);
                var driveType = isRemovable ? DriveType.Removable : drive.DriveType;
                var name = GetName(drive.Name, driveType);
                var glyph = GetGlyph(driveType);

                var model = new DriveViewModel(name, drive.RootDirectory.Root.FullName, glyph);
                driveModels.Add(model);
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
