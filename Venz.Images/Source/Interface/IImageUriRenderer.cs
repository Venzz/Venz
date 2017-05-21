using System;

namespace Venz.Images
{
    public interface IImageUriRenderer
    {
        ImageUriContent UriContent { set; }
        Object PictureRequestId { get; set; }
        Object Tag { get; set; }
    }
}
