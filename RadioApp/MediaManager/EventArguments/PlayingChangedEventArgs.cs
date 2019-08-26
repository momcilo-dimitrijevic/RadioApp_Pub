﻿using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class PlayingChangedEventArgs : EventArgs
    {
        public PlayingChangedEventArgs(double progress, TimeSpan position, TimeSpan duration)
        {
            Progress = progress;
            Position = position;
            Duration = duration;
        }
        public double Progress { get; }
        public TimeSpan Position { get; }
        public TimeSpan Duration { get; }
    }
}