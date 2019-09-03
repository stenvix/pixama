﻿using Pixama.Logic.ViewModels.Common;
using ReactiveUI;
using System.Reactive.Disposables;
using Windows.UI.Xaml;

namespace Pixama.App.Views.Photo
{
    public class FolderItemViewBase : ReactiveUserControl<FolderViewModel> { }
    public sealed partial class FolderItemView : FolderItemViewBase
    {
        public FolderItemView()
        {
            InitializeComponent();
            Loading += OnLoading;
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.Name,
                        v => v.FolderName.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Glyph,
                        v => v.Glyph.Glyph)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.ExpandGlyph,
                        v => v.ExpandGlyph.Glyph)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.Children,
                        v => v.Children.ItemsSource)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsSelected,
                        v => v.ListItem.IsSelected)
                    .DisposeWith(disposable);

                //Visibility
                this.OneWayBind(ViewModel,
                        vm => vm.HasUnrealizedChildren,
                        v => v.ExpandButton.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsLoading,
                        v => v.IsLoading.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsExpanded,
                        v => v.Children.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsStatic,
                        v => v.Glyph.Visibility)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        vm => vm.IsDynamic,
                        v => v.FavoriteButton.Visibility)
                    .DisposeWith(disposable);

                //Commands
                this.BindCommand(ViewModel,
                        vm => vm.ExpandCommand,
                        v => v.ExpandButton)
                    .DisposeWith(disposable);
            });
        }

        private async void OnLoading(FrameworkElement sender, object args)
        {
            await ViewModel.LoadAsync();
        }
    }
}
