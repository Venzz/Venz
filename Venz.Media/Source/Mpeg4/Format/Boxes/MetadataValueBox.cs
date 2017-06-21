using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// https://developer.apple.com/library/mac/documentation/QuickTime/QTFF/qtff.pdf (139)
    /// </summary>
    public class MetadataValueBox
    {
        public DataLocation DataLocation { get; private set; }

        private MetadataValueBox() { }

        public static Task<MetadataValueBox> CreateAsync(IBinaryStream stream, Box box)
        {
            var metadataValueBox = new MetadataValueBox();
            stream.Position = box.Offset + 8;
            stream.Position += 8; // TypeIndicator + LocaleIndicator
            metadataValueBox.DataLocation = new DataLocation(stream.Position, box.Length - stream.Position);
            return Task.FromResult(metadataValueBox);
        }
    }
}
