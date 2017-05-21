using System;
using Windows.Foundation;

namespace Venz.Images
{
    public class Picture
    {
        /// <summary>
        /// Shows, whether the picture content could be extracted.
        /// You can set this value to false to tell the image processor not to do anything with this picture.
        /// Default value is true.
        /// </summary>
        public virtual Boolean IsAvailable => true;

        /// <summary>
        /// Tells the image processor that this picture needs to be enqueued and will use async method of getting the content later on.
        /// If the value is false, this picture will be immediately rendered at the time the databinding evaluates.
        /// Default value is true.
        /// </summary>
        public virtual Boolean UseAsyncPattern => true;

        /// <summary>
        /// Represents width and height of the picture, if available.
        /// </summary>
        public virtual Size? Size { get; }



        protected Picture() { }
    }
}
