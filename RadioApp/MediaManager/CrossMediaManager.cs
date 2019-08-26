﻿using System;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    /// <summary>
    /// Cross MediaManager
    /// </summary>
    public static class CrossMediaManager
    {
        static Lazy<IMediaManager> implementation = new Lazy<IMediaManager>(() => CreateMediaManager(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        /// 
        //public static IMediaManager Current
        //{
        //    get
        //    {
        //        IMediaManager ret = implementation.Value;
        //        if (ret == null)
        //        {
        //            throw NotImplementedInReferenceAssembly();
        //        }
        //        return ret;
        //    }
        //}
        public static IMediaManager Current;

        static IMediaManager CreateMediaManager()
        {
#if NETSTANDARD
            return null;
#else
#pragma warning disable IDE0022 // Use expression body for methods
            return new MediaManagerImplementation();
#pragma warning restore IDE0022 // Use expression body for methods
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
