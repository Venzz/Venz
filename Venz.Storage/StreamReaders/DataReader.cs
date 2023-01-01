using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Venz.Storage
{
    public class DataReader: IDisposable
    {
        private Windows.Storage.Streams.DataReader Native;

        public DataReader(Windows.Storage.Streams.IInputStream stream)
        {
            Native = new Windows.Storage.Streams.DataReader(stream);
        }

        public String ReadString()
        {
            var stringDataSize = Native.ReadUInt16();
            if (stringDataSize == 0)
                return null;
            var stringData = Native.ReadBuffer(stringDataSize);
            return Encoding.UTF8.GetString(stringData.ToArray());
        }

        public Byte ReadByte()
        {
            return Native.ReadByte();
        }

        public UInt32 ReadUInt32()
        {
            return Native.ReadUInt32();
        }

        public UInt32? ReadNUInt32()
        {
            if (!Native.ReadBoolean())
                return null;
            return Native.ReadUInt32();
        }

        public DateTime ReadDateTime()
        {
            return new DateTime(Native.ReadInt64());
        }

        public Double ReadDouble()
        {
            return Native.ReadDouble();
        }

        public TimeSpan? ReadTimeSpan()
        {
            if (!Native.ReadBoolean())
                return null;
            return new TimeSpan(Native.ReadInt64());
        }

        public Byte[] ReadBytes(UInt32 count)
        {
            return Native.ReadBuffer(count).ToArray();
        }

        public Task LoadAsync(UInt32 count)
        {
            return Native.LoadAsync(count).AsTask();
        }

        public void Dispose()
        {
            Native.Dispose();
        }
    }
}
