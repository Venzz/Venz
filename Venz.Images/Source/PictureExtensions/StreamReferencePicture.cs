using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Venz.Images
{
    public class StreamReferencePicture: StreamPicture 
    {
        private RandomAccessStreamReference StreamReference;

        public StreamReferencePicture(RandomAccessStreamReference streamReference) { StreamReference = streamReference; }

        public override async Task<IRandomAccessStream> GetStreamAsync() => await StreamReference.OpenReadAsync();
    }
}
