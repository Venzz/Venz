using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Venz.Media.Metadata;
using Venz.Media.Metadata.Mp3;

namespace Venz.Media.Mp3
{
    // Frame header.
    // AAAAAAAA   AAABBCCD   EEEEFFGH   IIJJKLMM

    // A - Frame synchronizer. All bits are set to 1. It is used for finding the beginning of frame.
    // B - MPEG version ID (00 - MPEG Version 2.5, 01 - reserved, 10 - MPEG Version 2, 11 - MPEG Version 1). In most MP3 files these value should be 11.
    // C - Layer (00 - reserved, 01 - Layer III, 10 - Layer II, 11 - Layer I). In most MP3 files these value should be 01.
    // D - CRC Protection (0 - Protected by CRC, 1 - Not protected). CRC is 16 bit long and, if exists, it follows frame header. And then comes audio data. But almost all MP3 files doesnt contain CRC.
    // E - Bitrate index (0000 - free, 0001 - 32, 0010 - 40, 0011 - 48, 0100 - 56, 0101 - 64, 0110 - 80, 0111 - 96, 1000 - 112, 1001 - 128, 1010 - 160, 1011 - 192, 1100 - 224, 1101 - 256, 1110 - 320, 1111 - bad).
    // F - Samplig rate frequency index (00 - 44100, 01 - 48000, 10 - 32000, 11 - reserved). In most MP3 files these value should be 00. Note: Sample frequency 44100 means that one second of audio information is hacked to 44100 pieces. And each 1/44100 sec. is audio value taken and encoded into digital form.
    // G - Padding (0 - Frame is not padded, 1 - Frame is padded). Padding is used to fit the bitrates exactly. If you use frames with length 417 Bytes (for 128kbps) it doesnt give exact data flow 128kbps. So you can set Padding and add one extra Byte at the end of some frames to create exact 128kbps.
    // H - Private bit. It can be freely used for specific needs of an application, eg. it can execute some application specific events.
    // I - Channel (00 - Stereo, 01 - Joint Stereo, 10 - Dual, 11 - Mono).
    // J - Mode extension (00, 01, 10, 11) (only if Joint Stereo is set). Tells which mode for JointStereo is used.
    // K - Copyright (0 - Audio is not copyrighted, 1 - Audio is copyrighted).
    // L - Original (0 - Copy of original media, 1 - Original media).
    // M - Emphasis (00 - None, 01 - 50/15, 10 - reserved, 11 - CCIT J.17). Tells if there are emphasised frequencies above cca. 3.2 kHz.

    public class Mp3File
    {
        public IReadOnlyCollection<Frame> Samples { get; private set; } = new List<Frame>();
        public IMetadata Metadata { get; private set; }
        public BitrateType? Type { get; private set; }


 
        public static async Task<ParseResult<Mp3File>> ParseSamplesAsync(IBinaryStream stream)
        {
            var streamLength = await stream.GetLengthAsync().ConfigureAwait(false);
            stream.Position = 0;

            if (await Id3v2.IsPointingToHeaderAsync(stream).ConfigureAwait(false))
                await Id3v2.SkipAsync(stream).ConfigureAwait(false);

            var collectSamplesResult = await CollectSamplesAsync(stream, streamLength, null).ConfigureAwait(false);
            if (collectSamplesResult.Item1.Count == 0)
                return ParseResult<Mp3File>.CreateUnknownFormat();

            var file = new Mp3File();
            file.Type = collectSamplesResult.Item2;
            file.Samples = collectSamplesResult.Item1;
            return ParseResult<Mp3File>.Create(file);
        }

        public static async Task<ParseResult<Mp3File>> ParseMetadataAsync(IBinaryStream stream)
        {
            var streamLength = await stream.GetLengthAsync().ConfigureAwait(false);
            stream.Position = 0;

            var id3v2 = (Id3v2)null;
            if (await Id3v2.IsPointingToHeaderAsync(stream).ConfigureAwait(false))
            {
                id3v2 = await Id3v2.ParseAsync(stream).ConfigureAwait(false);
                return ParseResult<Mp3File>.Create(new Mp3File() { Metadata = new Mp3Metadata(id3v2) });
            }
            else
            {
                return ParseResult<Mp3File>.Create(new Mp3File() { Metadata = new Mp3Metadata(null) });
            }
        }

        public static async Task<ParseResult<Mp3File>> ParseFullMetadataAsync(IBinaryStream stream)
        {
            var streamLength = await stream.GetLengthAsync().ConfigureAwait(false);
            stream.Position = 0;

            var file = new Mp3File();
            var id3v2 = await Id3v2.IsPointingToHeaderAsync(stream).ConfigureAwait(false) ? await Id3v2.ParseAsync(stream).ConfigureAwait(false) : null;
            var collectSamplesResult = await CollectSamplesAsync(stream, streamLength, 1).ConfigureAwait(false);
            if (collectSamplesResult.Item1.Count == 0)
            {
                file.Metadata = new Mp3Metadata(id3v2);
                return ParseResult<Mp3File>.Create(file);
            }
            else
            {
                var firstFrame = collectSamplesResult.Item1[0];
                var contentLength = streamLength - stream.Position;
                if (collectSamplesResult.Item2 == BitrateType.Constant)
                {
                    var duration = TimeSpan.FromSeconds(Math.Ceiling((contentLength / firstFrame.Length) * firstFrame.Duration.TotalSeconds));
                    var bitrate = firstFrame.Bitrate;
                    file.Metadata = new Mp3Metadata(id3v2, bitrate, duration);
                }
                else
                {
                    var xingFrame = await XingFrame.CreateAsync(stream, firstFrame).ConfigureAwait(false);
                    if (xingFrame.FramesCount.HasValue)
                    {
                        var duration = TimeSpan.FromSeconds(xingFrame.FramesCount.Value * firstFrame.SamplesCount / firstFrame.Frequency);
                        var averageBitrate = (UInt32)(contentLength / duration.TotalSeconds) * 8;
                        file.Metadata = new Mp3Metadata(id3v2, averageBitrate, duration);
                    }
                    else
                    {
                        collectSamplesResult = await CollectSamplesAsync(stream, streamLength, null).ConfigureAwait(false);
                        file.Metadata = new Mp3Metadata(id3v2, collectSamplesResult.Item4, collectSamplesResult.Item3);
                    }
                }
                return ParseResult<Mp3File>.Create(file);
            }
        }
        
        private static async Task<Tuple<List<Frame>, BitrateType, TimeSpan, UInt32>> CollectSamplesAsync(IBinaryStream stream, UInt32 streamLength, UInt32? amount)
        {
            var samples = new List<Frame>();
            var bitrateType = BitrateType.Constant;
            var duration = new TimeSpan();
            var bitrateSum = 0U;
            while (stream.Position + 20 < streamLength)
            {
                if (amount.HasValue && (samples.Count == amount.Value))
                    break;

                var possibleFrameHeader = await stream.PeekUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
                if ((possibleFrameHeader & 0xFFFE0000) == 0xFFFA0000)
                {
                    var bitrate = Frame.GetBitrate(possibleFrameHeader);
                    var frequency = Frame.GetFrequency(possibleFrameHeader);
                    var padding = Frame.GetPadding(possibleFrameHeader);
                    var version = Frame.GetVersion(possibleFrameHeader);
                    var layer = Frame.GetLayer(possibleFrameHeader);
                    if ((bitrate != 0) && (frequency != 0))
                    {
                        var frame = new Frame(stream.Position, version, layer, bitrate, frequency, padding);
                        if (samples.Count == 0)
                        {
                            var firstFrameContent = await stream.ReadAsync(frame.Length).ConfigureAwait(false);
                            bitrateType = firstFrameContent.IndexOf("Xing").HasValue ? BitrateType.Variable : BitrateType.Constant;
                        }
                        else
                        {
                            stream.Position += frame.Length;
                            duration += frame.Duration;
                            bitrateSum += frame.Bitrate;
                        }
                        samples.Add(frame);
                        continue;
                    }
                }

                // this situation means that we've read some samples and haven't found valid sample right after the last valid sample
                // and it means that the file in wrong format, corrupted or we've found all samples.
                if (samples.Count > 0)
                    break;

                stream.Position += 1;
            }
            return new Tuple<List<Frame>, BitrateType, TimeSpan, UInt32>(samples, bitrateType, duration, (UInt32)(bitrateSum / samples.Count));
        }

        public enum BitrateType { Constant, Variable }
    }
}
