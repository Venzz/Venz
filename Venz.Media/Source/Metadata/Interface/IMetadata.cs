using System;

namespace Venz.Media.Metadata
{
    public interface IMetadata
    {
        String Title { get; }
        String Album { get; }
        String AlbumArtist { get; }
        String Artist { get; }
        UInt32? Track { get; }
        UInt32? Year { get; }
        DataLocation Cover { get; }
        UInt32? AverageBitrate { get; }
        TimeSpan? Duration { get; }
    }
}
