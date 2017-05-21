using System;
using System.Threading.Tasks;

namespace Venz.Images
{
    public class UriPicture: Picture
    {
        public virtual Uri GetUri() => null;
        public virtual Task<Uri> GetUriAsync() => Task.FromResult<Uri>(null);
    }
}
