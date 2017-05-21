using System;
using Windows.Graphics.Imaging;

namespace Venz.Extensions
{
    public static class BitmapEncoderExtensions
    {
        public static void SetScale(this BitmapEncoder source, UInt32 originalWidth, UInt32 originalHeight, UInt32 maxSideLength)
        {
            if (originalWidth > originalHeight)
            {
                if (originalWidth <= maxSideLength)
                    return;

                var factor = (Double)originalHeight / originalWidth;
                source.BitmapTransform.ScaledWidth = maxSideLength;
                source.BitmapTransform.ScaledHeight = (UInt32)(maxSideLength * factor);
            }
            else
            {
                if (originalHeight <= maxSideLength)
                    return;

                var factor = (Double)originalWidth / originalHeight;
                source.BitmapTransform.ScaledHeight = maxSideLength;
                source.BitmapTransform.ScaledWidth = (UInt32)(maxSideLength * factor);
            }
        }
    }
}
