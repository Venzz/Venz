using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// You use sample size atoms to specify the size of each sample in the media. Sample size atoms have an atom type of 'stsz'.
    /// </summary>
    public class SampleSizeBox
    {
        private Byte Version;
        private Int32 Flags;

        public IList<UInt32> Table { get; private set; }



        private SampleSizeBox()
        {
            Table = new List<UInt32>();
        }

        public static async Task<SampleSizeBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var stcoBox = new SampleSizeBox();
            stream.Position = box.Offset + 8;
            stcoBox.Version = await stream.ReadByteAsync().ConfigureAwait(false);
            stcoBox.Flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);

            var uniformSizeOfSample = await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            var entriesAmount = await stream.ReadInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            for (var i = 0; i < entriesAmount; i++)
                stcoBox.Table.Add(await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false));
            return stcoBox;
        }
    }
}
