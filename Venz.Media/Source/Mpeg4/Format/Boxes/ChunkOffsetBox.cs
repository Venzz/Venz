using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// Chunk offset atoms identify the location of each chunk of data in the media’s data stream. Chunk offset atoms have an atom type of 'stco'.
    /// </summary>
    public class ChunkOffsetBox
    {
        private Byte Version;
        private Int32 Flags;

        public IList<UInt32> Table { get; }



        private ChunkOffsetBox()
        {
            Table = new List<UInt32>();
        }

        public static async Task<ChunkOffsetBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var stcoBox = new ChunkOffsetBox();
            stream.Position = box.Offset + 8;
            stcoBox.Version = await stream.ReadByteAsync().ConfigureAwait(false);
            stcoBox.Flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);

            var entriesAmount = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            for (var i = 0; i < entriesAmount; i++)
                stcoBox.Table.Add(await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false));
            return stcoBox;
        }
    }
}
