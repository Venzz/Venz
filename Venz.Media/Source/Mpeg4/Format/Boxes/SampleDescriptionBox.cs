using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// STSD. The sample description atom stores information that allows you to decode samples in the media.
    /// The data stored in the sample description varies, depending on the media type. For example, in the case of video media, the sample descriptions are image description structures.
    /// The sample description information for each media type is explained in Media Data Atom Types
    /// </summary>
    public class SampleDescriptionBox
    {
        private Byte Version;
        private Int32 Flags;

        public IList<TableEntry> Table { get; private set; }



        private SampleDescriptionBox()
        {
            Table = new List<TableEntry>();
        }

        public static async Task<SampleDescriptionBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var stsdBox = new SampleDescriptionBox();
            stream.Position = box.Offset + 8;
            stsdBox.Version = await stream.ReadByteAsync().ConfigureAwait(false);
            stsdBox.Flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);

            var entriesAmount = await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            for (var i = 0; i < entriesAmount; i++)
            {
                var sampleDescriptionSize = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var dataFormat = await stream.ReadStringAsync(4).ConfigureAwait(false);
                stream.Position += 6;
                var dataReferenceIndex = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
                stsdBox.Table.Add(new TableEntry(sampleDescriptionSize, dataFormat, dataReferenceIndex));

                /*var version = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var revision = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var vendor = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var channels = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var sampleSize = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var compressionId = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var packetSize = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var sampleRate = await stream.ReadUInt16Async(ByteOrder.BigEndian).ConfigureAwait(false);*/
            }
            return stsdBox;
        }

        public class TableEntry
        {
            public UInt32 SampleDescriptionSize { get; private set; }
            public String DataFormat { get; private set; }
            public UInt16 DataReferenceIndex { get; private set; }

            public TableEntry(UInt32 sampleDescriptionSize, String dataFormat, UInt16 dataReferenceIndex)
            {
                SampleDescriptionSize = sampleDescriptionSize;
                DataFormat = dataFormat;
                DataReferenceIndex = dataReferenceIndex;
            }
        }
    }
}
