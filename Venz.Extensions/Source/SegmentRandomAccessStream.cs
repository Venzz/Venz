using System;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Venz.Extensions
{
    public sealed class SegmentRandomAccessStream: IRandomAccessStream
    {
        private IRandomAccessStream Stream;
        private UInt64 Offset;
        private UInt64 Length;

        public SegmentRandomAccessStream(IRandomAccessStream stream, UInt64 offset, UInt64 length)
        {
            Stream = stream;
            Offset = offset;
            Length = length;
            stream.Seek(offset);
        }

        public IInputStream GetInputStreamAt(UInt64 position) => Stream.GetInputStreamAt(Offset + position);

        public IOutputStream GetOutputStreamAt(UInt64 position) => Stream.GetOutputStreamAt(Offset + position);        

        public void Seek(UInt64 position) => Stream.Seek(Offset + position);

        public IRandomAccessStream CloneStream() => new SegmentRandomAccessStream(Stream.CloneStream(), Offset, Length);

        public Boolean CanRead => Stream.CanRead;

        public Boolean CanWrite => Stream.CanWrite;

        public UInt64 Position => Stream.Position - Offset;

        public UInt64 Size { get => Length; set => throw new NotImplementedException(); }

        public IAsyncOperationWithProgress<IBuffer, UInt32> ReadAsync(IBuffer buffer, UInt32 count, InputStreamOptions options) => Stream.ReadAsync(buffer, count, options);

        public IAsyncOperationWithProgress<UInt32, UInt32> WriteAsync(IBuffer buffer) => Stream.WriteAsync(buffer);

        public IAsyncOperation<Boolean> FlushAsync() => Stream.FlushAsync();

        public void Dispose() => Stream.Dispose();
    }
}
