using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venz.Media.Metadata;
using Venz.Media.Metadata.Mpeg4;

namespace Venz.Media.Mpeg4
{
    public class Mpeg4File
    {
        private IList<Box> Boxes = new List<Box>();

        public IMetadata Metadata { get; private set; }
        public Box this[String boxPath] => GetBox(boxPath);



        private Mpeg4File() { }

        public static async Task<ParseResult<Mpeg4File>> ParseAsync(IBinaryStream stream, ParseOptions options)
        {
            try
            {
                stream.Position = 4;
                if (await stream.ReadStringAsync(4) != "ftyp")
                    return ParseResult<Mpeg4File>.CreateUnknownFormat();

                var file = new Mpeg4File();
                stream.Position = 0;
                var streamLength = await stream.GetLengthAsync().ConfigureAwait(false);
                while (stream.Position < streamLength)
                    file.Boxes.Add(await Mpeg4File.ReadBoxAsync(stream, await stream.GetLengthAsync().ConfigureAwait(false)));

                if (options == ParseOptions.Metadata)
                {
                    var mediaData = file.Contains("mdat") ? file["mdat"] : null;
                    var metadataItems = file.Contains("moov.udta.meta.ilst") ? await MetadataItemsBox.CreateAsync(stream, file["moov.udta.meta.ilst"]).ConfigureAwait(false) : null;
                    var mediaHeader = file.Contains("moov.trak.mdia.mdhd") ? await MediaHeaderBox.CreateAsync(stream, file["moov.trak.mdia.mdhd"]).ConfigureAwait(false) : null;
                    file.Metadata = new Mpeg4Metadata(metadataItems?.Cover, mediaHeader?.Duration, mediaHeader?.TimeScale, mediaData?.Length);
                }

                return ParseResult<Mpeg4File>.Create(file);
            }
            catch (Exception exception)
            {
                return ParseResult<Mpeg4File>.Create(exception);
            }
        }

        public Boolean Contains(String boxPath)
        {
            var pathSteps = boxPath.Split('.');
            Box currentBox = null;
            foreach (var pathStep in pathSteps)
            {
                currentBox = (currentBox == null) ? Boxes.FirstOrDefault(a => a.Name == pathStep) : currentBox.Boxes.FirstOrDefault(a => a.Name == pathStep);
                if (currentBox == null)
                    return false;
            }
            return true;
        }

        public String ToStringView()
        {
            var view = "";
            foreach (var box in Boxes)
                view += box.ToStringView();
            return view;
        }

        private Box GetBox(String boxPath)
        {
            var pathSteps = boxPath.Split('.');
            Box currentBox = null;
            foreach (var pathStep in pathSteps)
            {
                currentBox = (currentBox == null) ? Boxes.FirstOrDefault(a => a.Name == pathStep) : currentBox.Boxes.FirstOrDefault(a => a.Name == pathStep);
                if (currentBox == null)
                    throw new ArgumentException($"Unable to find box \"{pathStep}\".");
            }
            return currentBox;
        }

        internal static async Task<Box> ReadBoxAsync(IBinaryStream stream, UInt32 maxPosition)
        {
            var boxes = new List<Box>();
            var offset = stream.Position;
            var length = await stream.ReadUInt32Async(ByteOrder.BigEndian).ConfigureAwait(false);
            if ((length < 8) || (stream.Position + length - 4 > maxPosition))
                return null;

            var name = await stream.ReadStringAsync(4).ConfigureAwait(false);
            if (name == "meta")
            {
                var version = await stream.ReadByteAsync().ConfigureAwait(false);
                var flags = await stream.ReadInt32Async(3, ByteOrder.BigEndian).ConfigureAwait(false);
            }
            var thisBoxMaxPosition = offset + length;
            while (stream.Position < thisBoxMaxPosition)
            {
                var box = await ReadBoxAsync(stream, thisBoxMaxPosition).ConfigureAwait(false);
                if (box == null)
                {
                    boxes.Clear();
                    stream.Position = thisBoxMaxPosition;
                    break;
                }
                boxes.Add(box);
            }
            return new Box(offset, length, name, boxes);
        }
    }
}
