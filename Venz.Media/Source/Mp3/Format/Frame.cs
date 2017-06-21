using System;

namespace Venz.Media.Mp3
{
    public class Frame
    {
        public UInt32 Offset { get; private set; }
        public UInt32 Length { get; private set; }
        public UInt32 Bitrate { get; private set; }
        public UInt16 Frequency { get; private set; }
        public UInt16 SamplesCount { get; private set; }
        public TimeSpan Duration { get; private set; }

        public Frame(UInt32 offset, UInt32 version, UInt32 layer, UInt32 bitrate, UInt16 frequency, Byte padding)
        {
            Offset = offset;
            Length = ((144 * bitrate / frequency) + padding);
            Bitrate = bitrate;
            Frequency = frequency;
            Duration = TimeSpan.FromSeconds((Double)(Length * 8) / Bitrate);
            switch (layer)
            {
                case 1:
                    SamplesCount = 384;
                    break;
                case 2:
                    SamplesCount = 1152;
                    break;
                default:
                    SamplesCount = (version == 1) ? (UInt16)1152 : (UInt16)576;
                    break;
            }
        }

        public static UInt32 GetBitrate(UInt32 frameHeader)
        {
            var bitRateId = (frameHeader & 0x0000F000) >> 12;
            switch (bitRateId)
            {
                case 1:
                    return 32000;
                case 2:
                    return 40000;
                case 3:
                    return 48000;
                case 4:
                    return 56000;
                case 5:
                    return 64000;
                case 6:
                    return 80000;
                case 7:
                    return 96000;
                case 8:
                    return 112000;
                case 9:
                    return 128000;
                case 10:
                    return 160000;
                case 11:
                    return 192000;
                case 12:
                    return 224000;
                case 13:
                    return 256000;
                case 14:
                    return 320000;
                case 0:
                case 15:
                default:
                    return 0;
            }
        }

        public static UInt16 GetFrequency(UInt32 frameHeader)
        {
            var frequencyId = (frameHeader & 0x00000C00) >> 10;
            switch (frequencyId)
            {
                case 0:
                    return 44100;
                case 1:
                    return 48000;
                case 2:
                    return 32000;
                case 3:
                default:
                    return 0;
            }
        }

        public static Byte GetPadding(UInt32 frameHeader)
        {
            return (Byte)((frameHeader & 0x00000200) >> 9);
        }

        public static UInt16 GetVersion(UInt32 frameHeader)
        {
            var versionId = (frameHeader & 0x00180000) >> 19;
            switch (versionId)
            {
                case 0:
                    return 25;
                case 2:
                    return 2;
                case 3:
                default:
                    return 1;
            }
        }

        public static UInt16 GetLayer(UInt32 frameHeader)
        {
            var layerId = (frameHeader & 0x00060000) >> 17;
            switch (layerId)
            {
                case 2:
                    return 2;
                case 3:
                    return 1;
                default:
                    return 3;
            }
        }
    }
}
