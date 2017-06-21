using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// Sample-to-chunk atoms have an atom type of 'stsc'. The sample-to-chunk atom contains a table that maps samples to chunks in the media data stream.
    /// By examining the sample-to-chunk atom, you can determine the chunk that contains a specific sample.
    /// </summary>
    public class SampleToChunkBox
    {
        private Byte Version;
        private Int32 Flags;

        public IList<TableEntry> Table { get; private set; }



        private SampleToChunkBox()
        {
            Table = new List<TableEntry>();
        }

        public static async Task<SampleToChunkBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var stscBox = new SampleToChunkBox();
            stream.Position = box.Offset + 8;
            stscBox.Version = await stream.ReadByteAsync().ConfigureAwait(false);
            stscBox.Flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);

            var entriesAmount = await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            for (var i = 0; i < entriesAmount; i++)
            {
                var firstChunk = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var samplesPerChunk = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var sampleDescriptionId = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                stscBox.Table.Add(new TableEntry(firstChunk, samplesPerChunk, sampleDescriptionId));
            }
            return stscBox;
        }

        public class TableEntry
        {
            public UInt32 FirstChunk { get; private set; }
            public UInt32 SamplesPerChunk { get; private set; }
            public UInt32 SampleDescriptionId { get; private set; }

            public TableEntry(UInt32 firstChunk, UInt32 samplesPerChunk, UInt32 sampleDescriptionId)
            {
                FirstChunk = firstChunk;
                SamplesPerChunk = samplesPerChunk;
                SampleDescriptionId = sampleDescriptionId;
            }
        }
    }
}
