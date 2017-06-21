using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// The metadata atom is the container for carrying metadata.
    /// https://developer.apple.com/library/mac/documentation/QuickTime/QTFF/qtff.pdf (129)
    /// </summary>
    public class MetadataBox
    {
        private Byte Version;
        private Int32 Flags;

        public ICollection<Box> Boxes { get; }



        private MetadataBox()
        {
            Boxes = new List<Box>();
        }

        public static async Task<MetadataBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var metadataBox = new MetadataBox();
            stream.Position = box.Offset + 8;
            metadataBox.Version = await stream.ReadByteAsync().ConfigureAwait(false);
            metadataBox.Flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);

            var boxes = new List<Box>();
            while (stream.Position < box.Offset + box.Length)
            {
                var subBox = await Mpeg4File.ReadBoxAsync(stream, box.Offset + box.Length).ConfigureAwait(false);
                if (subBox != null)
                    metadataBox.Boxes.Add(subBox);
            }
            return metadataBox;
        }
    }
}
