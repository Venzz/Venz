using System;

namespace Venz.Media.Metadata.Mp3
{
    internal class Mp3Metadata: IMetadata
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

        public Mp3Metadata(Id3v2 tag)
        {
            Title = tag?.Title;
            Album = tag?.Album;
            Artist = tag?.Artist;
            AlbumArtist = tag?.AlbumArtist;
            Track = tag?.Track;
            Year = tag?.Year;
            Cover = tag?.Cover;
        }

        public Mp3Metadata(Id3v2 tag, UInt32 averageBitrate, TimeSpan duration): this(tag)
        {
            Duration = duration;
            AverageBitrate = averageBitrate;
        }
    }
}
