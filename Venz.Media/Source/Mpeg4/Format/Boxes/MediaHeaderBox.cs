using System;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// The media header atom specifies the characteristics of a media, including time scale and duration. The media header atom has an atom type of 'mdhd'.
    /// </summary>
    public class MediaHeaderBox
    {
        private Byte Version;
        private Int32 Flags;

        public DateTime CreationTime { get; private set; }
        public DateTime ModificationTime { get; private set; }
        public Int32 TimeScale { get; private set; }
        public Int32 Duration { get; private set; }
        public Int16 Language { get; private set; }
        public Int16 Quality { get; private set; }



        private MediaHeaderBox()
        {
        }

        public static async Task<MediaHeaderBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var mdhdBox = new MediaHeaderBox();
            stream.Position = box.Offset + 8;
            mdhdBox.Version = await stream.ReadByteAsync().ConfigureAwait(false);
            mdhdBox.Flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);
            mdhdBox.CreationTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false)).ToLocalTime();
            mdhdBox.ModificationTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false)).ToLocalTime();
            mdhdBox.TimeScale = await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            mdhdBox.Duration = await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            mdhdBox.Language = await stream.ReadInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
            mdhdBox.Quality = await stream.ReadInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
            return mdhdBox;
        }
    }
}
