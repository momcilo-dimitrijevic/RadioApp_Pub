﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Net.Wifi;
using Android.Widget;
using Environment = Android.OS.Environment;

namespace Plugin.MediaManager.ExoPlayer
{
    using Android.Webkit;
    using Java.IO;
    using Java.Lang;
    using Java.Util.Concurrent;
    using Plugin.MediaManager.Abstractions.Enums;
    using Console = System.Console;
    using Double = System.Double;

    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop, ActionTogglePlayback, ActionNext, ActionPrevious })]
    public class ExoPlayerAudioService : MediaServiceBase, IPlayerEventListener, ExtractorMediaSource.IEventListener
    {
        private SimpleExoPlayer _mediaPlayer;

        private IScheduledExecutorService _executorService = Executors.NewSingleThreadScheduledExecutor();

        private IScheduledFuture _scheduledFuture;

        private static readonly DefaultBandwidthMeter BANDWIDTH_METER = new DefaultBandwidthMeter();

        public override TimeSpan Position
        {
            get
            {
                double parsedPosition;
                var position = _mediaPlayer?.CurrentPosition;
                if (position > 0 && Double.TryParse(position.ToString(), out parsedPosition))
                    return TimeSpan.FromMilliseconds(parsedPosition);
                return TimeSpan.Zero;
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                var duration = _mediaPlayer?.Duration;
                if (duration > 0 && Double.TryParse(duration.ToString(), out var parsedDuration))
                    return TimeSpan.FromMilliseconds(parsedDuration);
                return TimeSpan.Zero;
            }
        }

        public override TimeSpan Buffered
        {
            get
            {
                double parsedBuffering;
                var buffered = _mediaPlayer?.BufferedPosition;
                if (buffered > 0 && Double.TryParse(buffered.ToString(), out parsedBuffering))
                    return TimeSpan.FromMilliseconds(parsedBuffering);
                return TimeSpan.Zero;
            }
        }

        public override void InitializePlayer()
        {
            bool needNewPlayer = _mediaPlayer == null;
            if (needNewPlayer)
            {
                var adaptiveTrackSelectionFactory =
                    new AdaptiveTrackSelection.Factory(BANDWIDTH_METER);
                var trackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);
                var eventLogger = new ExoPlayerEventLogger(trackSelector);

                var renderersFactory = new DefaultRenderersFactory(ApplicationContext);
                var loadControl = new DefaultLoadControl();

                _mediaPlayer = ExoPlayerFactory.NewSimpleInstance(renderersFactory, trackSelector, loadControl);
                _mediaPlayer.AddListener(eventLogger);
                _mediaPlayer.AddListener(this);
                _mediaPlayer.SetAudioDebugListener(eventLogger);
                _mediaPlayer.SetMetadataOutput(eventLogger);
            }

            StatusChanged += (sender, args) => OnStatusChangedHandler(args);
        }

        public override void InitializePlayerWithUrl(string audioUrl)
        {
            throw new NotImplementedException();
        }

        public override void SetMediaPlayerOptions()
        {
        }

        public override async Task Play(IMediaFile mediaFile = null)
        {
            await base.Play(mediaFile);
            _mediaPlayer.PlayWhenReady = true;
            ManuallyPaused = false;
        }

        public override Task Seek(TimeSpan position)
        {
            return Task.Run(() =>
            {
                _mediaPlayer?.SeekTo(Convert.ToInt64(position.TotalMilliseconds));
            });
        }

        public override async Task Pause()
        {
            _mediaPlayer.PlayWhenReady = false;
            ManuallyPaused = true;
            await base.Pause();
        }

        public override async Task Stop()
        {
            _mediaPlayer.Stop();
            await base.Stop();
        }

        public override void SetVolume(float leftVolume, float rightVolume)
        {
            _mediaPlayer.Volume = leftVolume;
        }

        public override async Task<bool> SetMediaPlayerDataSource()
        {
            var source = GetSource(CurrentFile.Url);
            _mediaPlayer.Prepare(source);
            return await Task.FromResult(true);
        }

        protected override void Resume()
        {
        }

        #region ************ ExoPlayer Events *****************

        public void OnLoadingChanged(bool isLoading)
        {
            Console.WriteLine("Loading changed");
        }

        public void OnPlayerError(ExoPlaybackException ex)
        {
            OnMediaFailed(new MediaFailedEventArgs(ex.ToString(), ex));
        }

        public void OnPlayerStateChanged(bool playWhenReady, int state)
        {
            if (state == Com.Google.Android.Exoplayer2.ExoPlayer.StateEnded)
            {
                OnMediaFinished(new MediaFinishedEventArgs(CurrentFile));
            }
            else
            {
                var status = GetStatusByIntValue(state);
                var compatState = GetCompatValueByStatus(status);
                //OnStatusChanged(new StatusChangedEventArgs(status));
                SessionManager.UpdatePlaybackState(compatState, Position.Seconds);
            }
        }

        /* TODO: Implement IOutput Interface => https://github.com/martijn00/ExoPlayerXamarin/issues/38
         */
        //public void OnMetadata(Object obj)
        //{
        //    Console.WriteLine("OnMetadata");
        //}
        #endregion

        private MediaPlayerStatus GetStatusByIntValue(int state)
        {
            switch (state)
            {
                case Com.Google.Android.Exoplayer2.ExoPlayer.StateBuffering:
                    return MediaPlayerStatus.Buffering;
                case Com.Google.Android.Exoplayer2.ExoPlayer.StateReady:
                    return !ManuallyPaused && !TransientPaused ? MediaPlayerStatus.Playing : MediaPlayerStatus.Paused;
                case Com.Google.Android.Exoplayer2.ExoPlayer.StateIdle:
                    return MediaPlayerStatus.Stopped;
                default:
                    return MediaPlayerStatus.Failed;
            }
        }

        private int GetCompatValueByStatus(MediaPlayerStatus state)
        {
            switch (state)
            {
                case MediaPlayerStatus.Buffering:
                    return PlaybackStateCompat.StateBuffering;
                case MediaPlayerStatus.Failed:
                    return PlaybackStateCompat.StateError;
                case MediaPlayerStatus.Loading:
                    return PlaybackStateCompat.StateSkippingToQueueItem;
                case MediaPlayerStatus.Paused:
                    return PlaybackStateCompat.StatePaused;
                case MediaPlayerStatus.Playing:
                    return PlaybackStateCompat.StatePlaying;
                case MediaPlayerStatus.Stopped:
                    return PlaybackStateCompat.StateStopped;
                default:
                    return PlaybackStateCompat.StateNone;
            }
        }

        [Obsolete("deprecated")]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            HandleIntent(intent);
            return base.OnStartCommand(intent, flags, startId);
        }

        private void OnStatusChangedHandler(StatusChangedEventArgs args)
        {
            if (args.Status == MediaPlayerStatus.Buffering)
            {
                CancelBufferingTask();
                StartBufferingSchedule();
            }
            if (args.Status == MediaPlayerStatus.Stopped || args.Status == MediaPlayerStatus.Failed)
                CancelBufferingTask();
        }

        private void CancelBufferingTask()
        {
            _scheduledFuture?.Cancel(false);
            OnBufferingChanged(new BufferingChangedEventArgs(0, TimeSpan.Zero));
        }

        private void StartBufferingSchedule()
        {
            var handler = new Handler();
            var runnable = new Runnable(() => { handler.Post(OnBuffering); });
            if (!_executorService.IsShutdown)
                _scheduledFuture = _executorService.ScheduleAtFixedRate(runnable, 100, 1000, TimeUnit.Milliseconds);
        }

        private void OnBuffering()
        {
            if (_mediaPlayer == null || _mediaPlayer.BufferedPosition > TimeSpan.MaxValue.TotalMilliseconds - 100) return;

            OnBufferingChanged(
                new BufferingChangedEventArgs(
                    _mediaPlayer.BufferedPercentage,
                    TimeSpan.FromMilliseconds(Convert.ToInt64(_mediaPlayer.BufferedPosition))));
        }

        private IMediaSource GetSource(string url)
        {
            var factory = URLUtil.IsHttpUrl(url) || URLUtil.IsHttpsUrl(url) ? GetHttpFactory() : new FileDataSourceFactory();
            var uri = factory as FileDataSourceFactory != null ? Android.Net.Uri.Parse(Uri.EscapeDataString(url)) : Android.Net.Uri.Parse(url);
            var extractorFactory = new DefaultExtractorsFactory();
            return new ExtractorMediaSource(uri
                , factory
                , extractorFactory, null, this);
        }

        private IDataSourceFactory GetHttpFactory()
        {
            var bandwithMeter = new DefaultBandwidthMeter();
            var httpFactory = new DefaultHttpDataSourceFactory(Util.GetUserAgent(this, ApplicationInfo.Name), bandwithMeter);
            return new HttpSourceFactory(httpFactory, RequestHeaders);
        }

        public override Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            Console.WriteLine("Play: ");
            return null;
        }

        public void OnLoadError(IOException ex)
        {
            OnMediaFailed(new MediaFailedEventArgs(ex.ToString(), ex));
        }

        public void OnPlaybackParametersChanged(PlaybackParameters p0)
        {
            Console.WriteLine("OnPlaybackParametersChanged: " + p0);
        }

        public void OnPositionDiscontinuity(int p0)
        {
            Console.WriteLine("OnPositionDiscontinuity: " + p0);
        }

        public void OnRepeatModeChanged(int p0)
        {
            Console.WriteLine("OnRepeatModeChanged");
        }

        public void OnSeekProcessed()
        {
            Console.WriteLine("OnSeekProcessed");
        }

        public void OnShuffleModeEnabledChanged(bool p0)
        {
            Console.WriteLine("OnShuffleModeEnabledChanged");
        }

        public void OnTimelineChanged(Timeline p0, Object p1, int p2)
        {
            Console.WriteLine("OnTimelineChanged");
        }

        public void OnTracksChanged(TrackGroupArray p0, TrackSelectionArray p1)
        {
            Console.WriteLine("OnTracksChanged");
        }
    }

    public class HttpSourceFactory : Object, IDataSourceFactory
    {
        private DefaultHttpDataSourceFactory _httpFactory;
        private Dictionary<string, string> _headers;
        public HttpSourceFactory(DefaultHttpDataSourceFactory httpFactory, Dictionary<string, string> headers)
        {
            _httpFactory = httpFactory;
            _headers = headers;
        }

        public HttpSourceFactory()
        {
            Console.WriteLine("HSF called");
        }

        public IDataSource CreateDataSource()
        {
            var source = _httpFactory.CreateDataSource() as DefaultHttpDataSource;
            if (_headers == null || !_headers.Any())
                return source;

            foreach (var header in _headers)
            {
                source?.SetRequestProperty(header.Key, header.Value);
            }
            return source;
        }

        public IDataSource createDataSource()
        {
            return CreateDataSource();
        }
    }

}