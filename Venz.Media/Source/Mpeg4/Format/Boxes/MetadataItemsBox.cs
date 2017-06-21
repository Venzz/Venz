using System;
using System.Linq;
using System.Threading.Tasks;

namespace Venz.Media.Mpeg4
{
    /// <summary>
    /// https://developer.apple.com/library/mac/documentation/QuickTime/QTFF/qtff.pdf (129)
    /// </summary>
    public class MetadataItemsBox
    {
        public DataLocation Cover { get; private set; }

        private MetadataItemsBox() { }

        public static async Task<MetadataItemsBox> CreateAsync(IBinaryStream stream, Box box)
        {
            if (!box.IsContainer)
                throw new FormatException();

            var metadataItemsBox = new MetadataItemsBox();
            foreach (var subbox in box.Boxes)
            {
                switch (subbox.Name)
                {
                    case "covr":
                        var data = subbox.Boxes.FirstOrDefault(a => a.Name == "data");
                        if (data != null)
                            metadataItemsBox.Cover = (await MetadataValueBox.CreateAsync(stream, data).ConfigureAwait(false)).DataLocation;
                        break;
                    default:
                        break;
                }
            }
            return metadataItemsBox;
        }
    }
}
