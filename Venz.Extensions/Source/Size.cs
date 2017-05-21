using System;
using Windows.Foundation;

namespace Venz.Extensions
{
    public static class SizeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">Size that is used to inscribe current size with uniform rules.</param>
        public static Size GetUniformedSize(this Size value, Size availableSize)
        {
            if ((value.Width == 0) || (value.Height == 0))
                return value;

            if (value.Width < value.Height)
            {
                var factor = value.Width / value.Height;
                if (availableSize.Height * factor <= availableSize.Width)
                    return new Size(availableSize.Height * factor, availableSize.Height);
                else
                    return new Size(availableSize.Width, availableSize.Width * (value.Height / value.Width));
            }
            else
            {
                var factor = value.Height / value.Width;
                if (availableSize.Width * factor <= availableSize.Height)
                    return new Size(availableSize.Width, availableSize.Width * factor);
                else
                    return new Size(availableSize.Height * (value.Width / value.Height), availableSize.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">Size that is used to inscribe current size with uniform to fill rules.</param>
        public static Size GetUniformedToFillSize(this Size value, Size availableSize)
        {
            if ((value.Width <= availableSize.Width) || (value.Height <= availableSize.Height))
                return new Size(value.Width, value.Height);

            if (value.Width < value.Height)
            {
                var factor = value.Height / value.Width;
                return new Size(availableSize.Width, availableSize.Width * factor);
            }
            else
            {
                var factor = value.Width / value.Height;
                return new Size(availableSize.Height * factor, availableSize.Height);
            }
        }

        public static Boolean IsPortrait(this Size value) => value.Width < value.Height;

        public static Boolean IsLandscape(this Size value) => value.Width > value.Height;

        public static Boolean IsSquare(this Size value) => value.Width == value.Height;

        /// <summary>
        /// Shows whether the width and height are both greater than zero.
        /// </summary>
        public static Boolean IsVisible(this Size value) => (value.Width > 0) && (value.Height > 0);
    }
}
