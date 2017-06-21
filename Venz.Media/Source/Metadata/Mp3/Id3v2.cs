using System;
using System.Text;
using System.Threading.Tasks;

namespace Venz.Media.Metadata.Mp3
{
    internal class Id3v2
    {
        public String Title { get; private set; }
        public String Album { get; private set; }
        public String Artist { get; private set; }
        public String AlbumArtist { get; private set; }
        public UInt32? Track { get; private set; }
        public UInt32? Year { get; private set; }
        public DataLocation Cover { get; private set; }



        private Id3v2() { }

        public static async Task<Boolean> IsPointingToHeaderAsync(IBinaryStream stream) => await stream.PeekStringAsync(3).ConfigureAwait(false) == "ID3";

        public static async Task SkipAsync(IBinaryStream stream)
        {
            stream.Position += 3;
            var majorVersion = await stream.ReadByteAsync().ConfigureAwait(false);
            var revision = await stream.ReadByteAsync().ConfigureAwait(false);
            var flags = await stream.ReadByteAsync().ConfigureAwait(false);
            var size = Id3v2.DecodeSize(await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false));

            var newStreamPosition = stream.Position + size;
            if (await stream.GetLengthAsync().ConfigureAwait(false) < newStreamPosition)
                newStreamPosition = await stream.GetLengthAsync().ConfigureAwait(false);
            stream.Position = newStreamPosition;
        }

        public static async Task<Id3v2> ParseAsync(IBinaryStream stream)
        {
            stream.Position += 3;
            var id3v2 = new Id3v2();
            var majorVersion = await stream.ReadByteAsync().ConfigureAwait(false);
            var revision = await stream.ReadByteAsync().ConfigureAwait(false);
            var flags = await stream.ReadByteAsync().ConfigureAwait(false);
            var size = Id3v2.DecodeSize(await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false));
            var endingPosition = stream.Position + size;

            while (stream.Position < endingPosition)
            {
                var frameId = await stream.ReadStringAsync(4).ConfigureAwait(false);
                if (!Id3v2.IsValidAlphaNumeric(frameId))
                {
                    stream.Position = endingPosition;
                    break;
                }

                var frameSize = 0U;
                switch (majorVersion)
                {
                    case 4:
                        frameSize = Id3v2.DecodeSize(await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false));
                        break;
                    case 3:
                        frameSize = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                var frameFlags = await stream.ReadUInt16Async(ByteOrder.LittleEndian).ConfigureAwait(false);
                switch (frameId)
                {
                    case "TIT2":
                        id3v2.Title = Id3v2.DecodeString(await stream.ReadAsync(frameSize).ConfigureAwait(false));
                        break;
                    case "TALB":
                        id3v2.Album = Id3v2.DecodeString(await stream.ReadAsync(frameSize).ConfigureAwait(false));
                        break;
                    case "TOAL":
                        break;
                    case "TPE1":
                        id3v2.Artist = Id3v2.DecodeString(await stream.ReadAsync(frameSize).ConfigureAwait(false));
                        break;
                    case "TPE2":
                        id3v2.AlbumArtist = Id3v2.DecodeString(await stream.ReadAsync(frameSize).ConfigureAwait(false));
                        break;
                    case "TRCK":
                        var track = Id3v2.DecodeString(await stream.ReadAsync(frameSize).ConfigureAwait(false));
                        if (track != null)
                            id3v2.Track = ParseTrack(track);
                        break;
                    case "TYER":
                        var year = Id3v2.DecodeString(await stream.ReadAsync(frameSize).ConfigureAwait(false));
                        if (year != null)
                            id3v2.Year = ParseYear(year);
                        break;
                    case "APIC":
                        var readBytes = 0U;
                        var encoding = (FrameEncoding)await stream.ReadByteAsync().ConfigureAwait(false);
                        readBytes++;
                        while (await stream.ReadByteAsync().ConfigureAwait(false) != 0) // MIME type <text string> $00
                            readBytes++;
                        readBytes++;
                        var pictureType = await stream.ReadByteAsync().ConfigureAwait(false);
                        readBytes++;
                        while (await stream.ReadByteAsync().ConfigureAwait(false) != 0) // <text string according to encoding> $00 (00)
                            readBytes++;
                        id3v2.Cover = new DataLocation(stream.Position, frameSize - readBytes - 1);
                        stream.Position += frameSize - readBytes - 1;
                        break;
                    case "TCON":
                    case "COMM":
                    default:
                        stream.Position += frameSize;
                        break;
                }
            }

            return id3v2;
        }

        public static UInt32 EncodeSize(UInt32 size)
        {
            if (size > 0x0FFFFFFF)
                throw new ArgumentException("Specified size is too large.");

            UInt32 temp = ((size & 0x0FFFFF80) << 1);
            size &= 0x0000007F;
            size |= temp;
            temp = ((size & 0x0FFF8000) << 1);
            size &= 0x00007FFF;
            size |= temp;
            temp = ((size & 0x0F800000) << 1);
            size &= 0x007FFFFF;
            size |= temp;
            return size;
        }

        public static UInt32 DecodeSize(UInt32 size)
        {
            var newSize = 0U;
            var pointer = 0;
            for (var i = 0; i < 32; i++)
            {
                if (i % 8 == 7)
                    continue;
                newSize |= (UInt32)((size & (0x1 << i)) >> i << pointer++);
            }
            return newSize;
        }

        private static String DecodeString(Byte[] data)
        {
            if (data.Length < 2)
                return null;

            switch ((FrameEncoding)data[0])
            {
                case FrameEncoding.ASCII:
                case FrameEncoding.UTF8:
                    return Encoding.UTF8.GetString(data, 1, data.Length - 1).Replace("\0", "");
                case FrameEncoding.UTF16LE:
                    return Encoding.Unicode.GetString(data, 3, data.Length - 3).Replace("\0", "");
                case FrameEncoding.UTF16BE:
                    return Encoding.BigEndianUnicode.GetString(data, 1, data.Length - 1).Replace("\0", "");
                default:
                    throw new NotSupportedException();
            }
        }

        private static Boolean IsValidAlphaNumeric(String value)
        {
            foreach (Char character in value)
                if (!Char.IsLetterOrDigit(character))
                    return false;
            return true;
        }

        private static UInt32? ParseTrack(String track)
        {
            try
            {
                var number = "";
                foreach (var character in track.Trim())
                {
                    if (!Char.IsDigit(character))
                        break;
                    number += character;
                }
                return (number.Length == 0) ? (UInt32?)null : UInt32.Parse(number);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(track, exception);
            }
        }

        private static UInt32? ParseYear(String year)
        {
            try
            {
                if (year.Length > 4)
                    return DateTime.TryParse(year, out DateTime value) ? (UInt32)value.Year : (UInt32?)null;
                else if (year.Length == 4)
                    return UInt32.TryParse(year, out UInt32 value) ? value : (UInt32?)null;
                else
                    return null;
            }
            catch (Exception exception)
            {
                throw new ArgumentException(year, exception);
            }
        }



        private enum FrameEncoding
        {
            /// <summary>
            /// $00 – ISO-8859-1 (ASCII).
            /// </summary>
            ASCII = 0,

            /// <summary>
            /// $01 – UCS-2 in ID3v2.2 and ID3v2.3, UTF-16 encoded Unicode with BOM.
            /// </summary>
            UTF16LE = 1,

            /// <summary>
            /// $02 – UTF-16BE encoded Unicode without BOM in ID3v2.4 only.
            /// </summary>
            UTF16BE = 2,

            /// <summary>
            /// $03 – UTF-8 encoded Unicode in ID3v2.4 only.
            /// </summary>
            UTF8 = 3,
        }
    }
}
