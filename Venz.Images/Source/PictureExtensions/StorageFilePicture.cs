using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Venz.Images
{
    public class StorageFilePicture: StreamPicture 
    {
        private StorageFile File;

        public Boolean UseThumbnail { get; set; }



        public StorageFilePicture(StorageFile file)
        {
            File = file;
        }

        public override async Task<IRandomAccessStream> GetStreamAsync()
        {
            return UseThumbnail ? await File.GetThumbnailAsync(ThumbnailMode.SingleItem).AsTask().ConfigureAwait(false) : await File.OpenReadAsync().AsTask().ConfigureAwait(false);
        }
    }
}
