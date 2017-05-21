using System;
using Windows.Graphics.Imaging;

namespace Venz.Extensions
{
    public static class BitmapDecoderExtensions
    {
        public static Boolean FitInsideBox(this BitmapDecoder source, UInt32 maxSideLength)
        {
            if (source.PixelWidth > source.PixelHeight)
                return source.PixelWidth <= maxSideLength;
            else
                return source.PixelHeight <= maxSideLength;
        }
    }
}
