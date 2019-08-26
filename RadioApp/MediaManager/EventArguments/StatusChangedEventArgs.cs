﻿using System;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(MediaPlayerStatus status)
        {
            Status = status;
        }
        public MediaPlayerStatus Status { get; }
    }
}