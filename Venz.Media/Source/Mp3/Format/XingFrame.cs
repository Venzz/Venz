using System;
using System.Threading.Tasks;

namespace Venz.Media.Mp3
{
    public class XingFrame
    {
        public UInt32? FramesCount { get; private set; }
        public UInt32? BytesCount { get; private set; }
        public UInt32? Quality { get; private set; }

        private XingFrame() { }

        public static async Task<XingFrame> CreateAsync(IBinaryStream stream, Frame frame)
        {
            var position = stream.Position;
            stream.Position = frame.Offset;
            var data = await stream.ReadAsync(40).ConfigureAwait(false);
            var index = data.IndexOf("Xing");
            stream.Position = frame.Offset + index.Value + 4;

            var framesCount = (UInt32?)null;
            var bytesCount = (UInt32?)null;
            var toc = (Byte[])null;
            var quality = (UInt32?)null;
            var flags = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            if ((flags & 0x01) > 0)
                framesCount = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            if ((flags & 0x02) > 0)
                bytesCount = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            if ((flags & 0x04) > 0)
                toc = await stream.ReadAsync(100).ConfigureAwait(false);
            if ((flags & 0x08) > 0)
                quality = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);

            stream.Position = position;
            return new XingFrame() { FramesCount = framesCount, BytesCount = bytesCount, Quality = quality };
        }
    }
}
