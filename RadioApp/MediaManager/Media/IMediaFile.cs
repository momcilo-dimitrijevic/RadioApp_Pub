using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions
{
    public delegate void MetadataUpdatedEventHandler(object sender, MetadataChangedEventArgs e);

    /// <summary>
    /// Information about the mediafile
    /// </summary>
    public interface IMediaFile
    {
        /// <summary>
        /// Indicator for player which type of file it should play
        /// </summary>
        MediaFileType Type { get; set; }

        /// <summary>
        /// Indicates wether the resource is locally or remotely available
        /// </summary>
        ResourceAvailability Availability { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        IMediaFileMetadata Metadata { get; set; }

        /// <summary>
        /// Raised when mediadata of MediaFile failed to update
        /// </summary>
        event MetadataUpdatedEventHandler MetadataUpdated;

        /// <summary>
        /// Gets or sets the URL. Can be on the internet or local storage
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [metadata extracted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [metadata extracted]; otherwise, <c>false</c>.
        /// </value>
        bool MetadataExtracted { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether meta data should be extracted and overwrite user meta data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if overwrite user meta data; otherwise, <c>false</c>.
        /// </value>
        bool ExtractMetadata { get; set; }
    }
}

