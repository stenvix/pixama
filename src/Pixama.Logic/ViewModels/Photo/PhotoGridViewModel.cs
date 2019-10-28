using DynamicData;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Common;
using Pixama.Logic.ViewModels.Events;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Pixama.Logic.ViewModels.Photo
{
    public class PhotoGridViewModel : BaseViewModel
    {
        #region Fields

        private readonly IPhotoService _photoService;
        private readonly SourceList<PhotoGridItemViewModel> _photoList;
        private readonly ReadOnlyObservableCollection<PhotoGridItemViewModel> _photos;
        private readonly ObservableAsPropertyHelper<bool> _freshLoading;
        private readonly ObservableAsPropertyHelper<bool> _sourceSelected;
        private readonly ObservableAsPropertyHelper<int> _totalCount;
        private readonly ObservableAsPropertyHelper<int> _selectedCount;
        private readonly ObservableAsPropertyHelper<string> _counterText;
        private BaseLocationViewModel _sourceLocation;

        #endregion

        #region Properties

        public bool FreshLoading => _freshLoading.Value;
        public bool SourceSelected => _sourceSelected.Value;
        public int TotalCount => _totalCount.Value;
        public int SelectedCount => _selectedCount.Value;
        public string CounterText => _counterText.Value;
        public BaseLocationViewModel SourceLocation { get => _sourceLocation; set => this.RaiseAndSetIfChanged(ref _sourceLocation, value); }
        public ReadOnlyObservableCollection<PhotoGridItemViewModel> Photos => _photos;

        public ReactiveCommand<PhotoGridItemViewModel, Unit> ItemClickCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }
        public ReactiveCommand<Unit, Unit> DeselectAllCommand { get; }

        #endregion

        public PhotoGridViewModel(IPhotoService photoService)
        {
            _photoService = photoService;

            _photoList = new SourceList<PhotoGridItemViewModel>();
            _photoList.Connect()
                .ObserveOnDispatcher()
                .Bind(out _photos)
                .Subscribe();

            _photoList.CountChanged
                .ToProperty(this, i => i.TotalCount, out _totalCount);

            _photoList.Connect()
                .AutoRefresh(i => i.IsChecked, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500))
                .Filter(i => i.IsChecked)
                .ObserveOn(RxApp.MainThreadScheduler)
                .AsObservableList()
                .CountChanged
                .ToProperty(this, vm => vm.SelectedCount, out _selectedCount);

            this.WhenAnyValue(i => i.IsLoading, i => i.SourceLocation,
                    (isLoading, location) => !isLoading && location != null)
                .ToProperty(this, x => x.SourceSelected, out _sourceSelected);

            this.WhenAnyValue(i => i.IsLoading, i => i.SourceSelected,
                    (isLoading, sourceSelected) => !isLoading && !sourceSelected)
                .ToProperty(this, x => x.FreshLoading, out _freshLoading);

            this.WhenAnyValue(i => i.SelectedCount, i => i.TotalCount, (selected, total) => $"{selected} of {total}")
                .ToProperty(this, x => x.CounterText, out _counterText);


            MessageBus.Current.ListenIncludeLatest<LocationChanged>().Subscribe(OnLocationChanged);
            MessageBus.Current.Listen<SourceLocationRemoved>().Subscribe(OnSourceLocationRemoved);

            ItemClickCommand = ReactiveCommand.CreateFromTask<PhotoGridItemViewModel>(OnItemClickAsync);
            SelectAllCommand = ReactiveCommand.Create(OnSelectAll);
            DeselectAllCommand = ReactiveCommand.Create(OnDeselectAll);
        }

        private void OnSelectAll()
        {
            _photoList.Edit(list =>
            {
                foreach (var model in list)
                {
                    model.IsChecked = true;
                }
            });
            MessageBus.Current.SendMessage(new SelectAllPhotos());
        }

        private void OnDeselectAll()
        {
            _photoList.Edit(list =>
            {
                foreach (var model in list)
                {
                    model.IsChecked = false;
                }
            });
            MessageBus.Current.SendMessage(new DeselectAllPhotos());
        }

        private Task OnItemClickAsync(PhotoGridItemViewModel arg)
        {
            if (arg == null) return Task.CompletedTask;
            arg.IsChecked = !arg.IsChecked;
            return Task.CompletedTask;
        }


        private async void OnLocationChanged(LocationChanged args)
        {
            if (args?.Location == null || SourceLocation != null && SourceLocation == args.Location) return;

            IsLoading = true;

            if (await _photoService.GetFilesFromFolderAsync(args.Location.StorageFolder, _photoList))
            {
                SourceLocation = args.Location;
            }
            else
            {
                SourceLocation = null;
            }
            IsLoading = false;
        }

        private void OnSourceLocationRemoved(SourceLocationRemoved args)
        {
            if (args?.Location == null) return;
            if (SourceLocation.StorageFolder.Path.Contains(args.Location.StorageFolder.Path))
            {
                SourceLocation = null;
                _photoList.Clear();
            }
        }
    }
}
