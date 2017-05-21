using Windows.Foundation;
using Windows.Storage.Streams;

namespace Venz.Images
{
    public sealed class ImageStreamContent
    {
        public Size? Size { get; }
        public IRandomAccessStream Stream { get; }

        internal ImageStreamContent(Size? size, IRandomAccessStream stream)
        {
            Size = size;
            Stream = stream;
        }
    }
}
