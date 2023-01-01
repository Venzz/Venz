using System;
using System.Text;
using System.Threading.Tasks;

namespace Venz.Media
{
    internal static class BinaryStreamExtensions
    {
        public static async Task<Byte> ReadByteAsync(this IBinaryStream stream)
        {
            var buffer = await stream.ReadAsync(1).ConfigureAwait(false);
            return buffer[0];
        }

        public static async Task<Int16> ReadInt16Async(this IBinaryStream stream, ByteOrder byteOrder)
        {
            var buffer = await stream.ReadAsync(2).ConfigureAwait(false);
            return (Int16)((byteOrder == ByteOrder.BigEndian) ? ((buffer[0] << 8) | buffer[1]) : (buffer[0] | (buffer[1] << 8)));
        }

        public static async Task<UInt16> ReadUInt16Async(this IBinaryStream stream, ByteOrder byteOrder)
        {
            var buffer = await stream.ReadAsync(2).ConfigureAwait(false);
            return (UInt16)((byteOrder == ByteOrder.BigEndian) ? ((buffer[0] << 8) | buffer[1]) : (buffer[0] | (buffer[1] << 8)));
        }

        public static Task<Int32> ReadInt32Async(this IBinaryStream stream, ByteOrder byteOrder)
        {
            return ReadInt32Async(stream, 4, byteOrder);
        }

        public static async Task<Int32> ReadInt32Async(this IBinaryStream stream, Byte amount, ByteOrder byteOrder)
        {
            if (amount > 4)
                throw new ArgumentOutOfRangeException();

            var buffer = await stream.ReadAsync(amount).ConfigureAwait(false);
            var value = 0;
            for (var i = 0; i < amount; i++)
                value |= buffer[i] << 8 * ((byteOrder == ByteOrder.BigEndian) ? (amount - i - 1) : i);
            return value;
        }

        public static async Task<UInt32> ReadUInt32Async(this IBinaryStream stream, ByteOrder byteOrder)
        {
            var buffer = await stream.ReadAsync(4).ConfigureAwait(false);
            var value = 0U;
            for (var i = 0; i < 4; i++)
                value |= (UInt32)buffer[i] << 8 * ((byteOrder == ByteOrder.BigEndian) ? (3 - i) : i);
            return value;
        }

        public static async Task<UInt32> PeekUInt32Async(this IBinaryStream stream, ByteOrder byteOrder)
        {
            var value = await ReadUInt32Async(stream, byteOrder).ConfigureAwait(false);
            stream.Position -= 4;
            return value;
        }

        public static async Task<String> ReadStringAsync(this IBinaryStream stream, UInt32 length)
        {
            var buffer = await stream.ReadAsync(length).ConfigureAwait(false);
            return Encoding.UTF8.GetString(buffer, 0, (Int32)length);
        }

        public static async Task<String> PeekStringAsync(this IBinaryStream stream, UInt32 length)
        {
            var value = await ReadStringAsync(stream, length).ConfigureAwait(false);
            stream.Position -= length;
            return value;
        }
    }

    internal enum ByteOrder
    {
        /// <summary>
        /// 258 = 0x0102
        /// </summary>
        BigEndian,

        /// <summary>
        /// 258 = 0x0201
        /// </summary>
        LittleEndian
    }
}
