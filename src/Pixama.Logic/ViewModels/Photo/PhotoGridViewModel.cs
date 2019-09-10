using DynamicData;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Events;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Pixama.Logic.ViewModels.Common;

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
        private BaseLocationViewModel _sourceLocation;

        #endregion

        #region Properties

        public bool FreshLoading => _freshLoading.Value;
        public bool SourceSelected => _sourceSelected.Value;
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

            this.WhenAnyValue(i => i.IsLoading, i => i.SourceLocation,
                    (isLoading, location) => !isLoading && location != null)
                .ToProperty(this, x => x.SourceSelected, out _sourceSelected);

            this.WhenAnyValue(i => i.IsLoading, i => i.SourceSelected,
                    (isLoading, sourceSelected) => !isLoading && !sourceSelected)
                .ToProperty(this, x => x.FreshLoading, out _freshLoading);


            MessageBus.Current.ListenIncludeLatest<LocationChanged>().Subscribe(OnLocationChanged);

            ItemClickCommand = ReactiveCommand.CreateFromTask<PhotoGridItemViewModel>(OnItemClickAsync);
            SelectAllCommand = ReactiveCommand.Create(OnSelectAll);
            DeselectAllCommand = ReactiveCommand.Create(OnDeselectAll);
        }

        private void OnSelectAll() => MessageBus.Current.SendMessage(new SelectAllPhotos());
        private void OnDeselectAll() => MessageBus.Current.SendMessage(new DeselectAllPhotos());

        private Task OnItemClickAsync(PhotoGridItemViewModel arg)
        {
            arg.IsChecked = !arg.IsChecked;
            return Task.CompletedTask;
        }


        private async void OnLocationChanged(LocationChanged args)
        {
            if (args?.Location == null) return;
            SourceLocation = args.Location;
            IsLoading = true;
            await _photoService.GetFilesFromFolderAsync(args.Location.StorageFolder, _photoList);
            IsLoading = false;
        }
    }
}
