using System;

namespace Venz.Media.Metadata.Mpeg4
{
    internal class Mpeg4Metadata: IMetadata
    {
        public String Title { get; }
        public String Album { get; }
        public String Artist { get; }
        public String AlbumArtist { get; }
        public UInt32? Track { get; }
        public UInt32? Year { get; }
        public DataLocation Cover { get; }
        public UInt32? AverageBitrate { get; }
        public TimeSpan? Duration { get; }

        public Mpeg4Metadata(DataLocation cover, Int32? duration, Int32? timescale, UInt32? length)
        {
            Cover = cover;
            if (duration.HasValue && timescale.HasValue)
                Duration = TimeSpan.FromSeconds((Double)duration.Value / timescale.Value);
            if (length.HasValue && Duration.HasValue)
                AverageBitrate = (UInt32)(length.Value * 8 / Duration.Value.TotalSeconds);
        }
    }
}
