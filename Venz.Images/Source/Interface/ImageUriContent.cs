using System;
using Windows.Foundation;

namespace Venz.Images
{
    public sealed class ImageUriContent
    {
        public Size? Size { get; }
        public Uri Uri { get; }

        internal ImageUriContent(Size? size, Uri uri)
        {
            Size = size;
            Uri = uri;
        }
    }
}
