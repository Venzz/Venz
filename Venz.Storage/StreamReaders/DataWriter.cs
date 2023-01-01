using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Venz.Storage
{
    public class DataWriter: IDisposable
    {
        private Windows.Storage.Streams.DataWriter Native;

        public DataWriter(Windows.Storage.Streams.IOutputStream stream)
        {
            Native = new Windows.Storage.Streams.DataWriter(stream);
        }

        public void Write(String value)
        {
            var valueData = String.IsNullOrEmpty(value) ? new Byte[0] : Encoding.UTF8.GetBytes(value);
            Native.WriteUInt16((UInt16)valueData.Length);
            if (valueData.Length == 0)
                return;
            Native.WriteBuffer(valueData.AsBuffer());
        }

        public void Write(Byte value)
        {
            Native.WriteByte(value);
        }

        public void Write(Int32 value)
        {
            Native.WriteInt32(value);
        }

        public void Write(UInt32 value)
        {
            Native.WriteUInt32(value);
        }

        public void Write(UInt32? value)
        {
            Native.WriteBoolean(value.HasValue);
            if (!value.HasValue)
                return;
            Native.WriteUInt32(value.Value);
        }

        public void Write(DateTime value)
        {
            Native.WriteInt64(value.Ticks);
        }

        public void Write(Double value)
        {
            Native.WriteDouble(value);
        }

        public void Write(TimeSpan? value)
        {
            Native.WriteBoolean(value.HasValue);
            if (!value.HasValue)
                return;
            Native.WriteInt64(value.Value.Ticks);
        }

        public Task StoreAsync()
        {
            return Native.StoreAsync().AsTask();
        }

        public void Dispose()
        {
            Native.Dispose();
        }
    }
}
