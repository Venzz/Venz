using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// Time-to-sample atoms store duration information for a media’s samples, providing a mapping from a time in a media to the corresponding data sample.
    /// The time-to-sample atom has an atom type of 'stts'.
    /// </summary>
    public class TimeToSampleBox
    {
        private Byte Version;
        private Int32 Flags;

        public IList<TableEntry> Table { get; private set; }



        private TimeToSampleBox()
        {
            Table = new List<TableEntry>();
        }

        public static async Task<TimeToSampleBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var sttsBox = new TimeToSampleBox();
            stream.Position = box.Offset + 8;
            sttsBox.Version = await stream.ReadByteAsync().ConfigureAwait(false);
            sttsBox.Flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);

            var entriesAmount = await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            for (var i = 0; i < entriesAmount; i++)
            {
                var sampleCount = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                var sampleDuration = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                sttsBox.Table.Add(new TableEntry(sampleCount, sampleDuration));
            }
            return sttsBox;
        }

        public class TableEntry
        {
            /// <summary>
            /// Specifies the number of consecutive samples that have the same duration.
            /// </summary>
            public UInt32 SampleCount { get; private set; }

            /// <summary>
            /// Specifies the duration of each sample.
            /// </summary>
            public UInt32 SampleDuration { get; private set; }

            public TableEntry(UInt32 sampleCount, UInt32 sampleDuration)
            {
                SampleCount = sampleCount;
                SampleDuration = sampleDuration;
            }
        }
    }
}
