using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Venz.Images
{
    public class StreamPicture: Picture
    {
        public virtual IRandomAccessStream GetStream() => null;
        public virtual Task<IRandomAccessStream> GetStreamAsync() => Task.FromResult<IRandomAccessStream>(null);
    }
}
