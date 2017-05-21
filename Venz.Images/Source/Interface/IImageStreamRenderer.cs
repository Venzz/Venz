using System;

namespace Venz.Images
{
    public interface IImageStreamRenderer
    {
        ImageStreamContent StreamContent { set; }
        Object PictureRequestId { get; set; }
        Object Tag { get; set; }
    }
}
