using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using MediaManager.Forms;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;
using RadioApp.Models;
using RadioApp.Services;
using RadioApp.ViewModels.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace RadioApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly ReactiveMediaManager _mediaManager;

        private readonly ObservableAsPropertyHelper<bool> _isBusy;
        public bool IsBusy => _isBusy?.Value ?? false;

        private readonly ObservableAsPropertyHelper<MediaPlayerStatus> _playbackStatus;
        public MediaPlayerStatus PlaybackStatus => _playbackStatus?.Value ?? MediaPlayerStatus.Stopped;

        private readonly IStationService _stationService;

        private readonly SourceList<StationModel> _stationSource;
        public ReadOnlyObservableCollection<StationModel> BindingData;

        [Reactive]
        public string SearchTerm { get; set; }

        public ReactiveCommand<string, Unit> PlayCommand { get; private set; }

        public HomeViewModel(IStationService stationService = null)
        {
            UrlPathSegment = "RadioApp";
            _stationSource = new SourceList<StationModel>();

            _stationService = stationService ?? Locator.Current.GetService<IStationService>();
            _mediaManager = new ReactiveMediaManager();

            PlayCommand = ReactiveCommand.CreateFromTask<string>(async playlistUrl =>
            {
                var streamUrl = await _stationService.GetStreamUrl(playlistUrl);
                await _mediaManager.Play(streamUrl);
            });

            _stationSource
                .Connect()
                .Filter(this.WhenValueChanged(x => x.SearchTerm)
                            .Throttle(TimeSpan.FromMilliseconds(500))
                            .Select(StationNamePredicate))
                .Sort(SortExpressionComparer<StationModel>.Ascending(t => t.Name))
                .ObserveOn(RxApp.MainThreadScheduler) // Make sure this is only right before the Bind()
                .Bind(out BindingData)
                .Subscribe();

            this.WhenAnyObservable(x => x._mediaManager.PlaybackState)
                .Select(x => x == MediaPlayerStatus.Buffering)
                .ToProperty(this, x => x.IsBusy, out _isBusy);

            this.WhenAnyObservable(x => x._mediaManager.PlaybackState)
                .ToProperty(this, x => x.PlaybackStatus, out _playbackStatus);

            InitializeDataAsync();
        }

        private Func<StationModel, bool> StationNamePredicate(string filter)
        {
            return station => string.IsNullOrWhiteSpace(filter) ||
                              (station.Name?.ToLowerInvariant().Contains(filter.ToLowerInvariant()) ?? false);
        }

        private void InitializeDataAsync()
        {
            Task.Run(() => _stationService.Get())
                .ContinueWith(res => _stationSource.AddRange(res.Result));
        }
    }
}
