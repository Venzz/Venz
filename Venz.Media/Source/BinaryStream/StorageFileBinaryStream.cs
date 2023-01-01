using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Venz.Media
{
    public class StorageFileBinaryStream: IBinaryStream
    {
        private String Path;
        private StorageFile File;
        private Lazy<Task<IRandomAccessStream>> FileStreamTask;

        public UInt32 Position { get; set; }



        public StorageFileBinaryStream(String path)
        {
            Path = path;
            FileStreamTask = new Lazy<Task<IRandomAccessStream>>(() => GetStorageFileStreamAsync(), isThreadSafe: true);
        }

        public StorageFileBinaryStream(StorageFile file)
        {
            File = file;
            FileStreamTask = new Lazy<Task<IRandomAccessStream>>(() => GetStorageFileStreamAsync(), isThreadSafe: true);
        }

        public async Task<Byte[]> ReadAsync(UInt32 count)
        {
            var fileStream = await FileStreamTask.Value.ConfigureAwait(false);
            fileStream.Seek(Position);
            var buffer = await fileStream.ReadAsync(new Windows.Storage.Streams.Buffer(count), count, InputStreamOptions.Partial).AsTask().ConfigureAwait(false);
            Position += count;
            return buffer.ToArray();
        }

        public async Task<UInt32> GetLengthAsync()
        {
            var fileStream = await FileStreamTask.Value.ConfigureAwait(false);
            return (UInt32)fileStream.Size;
        }

        public void Dispose()
        {
            if (FileStreamTask.IsValueCreated)
                FileStreamTask.Value.ContinueWith((task) => task.Result.Dispose());
        }

        private async Task<IRandomAccessStream> GetStorageFileStreamAsync()
        {
            var storageFile = (File == null) ? await StorageFile.GetFileFromPathAsync(Path).AsTask().ConfigureAwait(false) : File;
            return await storageFile.OpenReadAsync().AsTask().ConfigureAwait(false);
        }
    }
}
