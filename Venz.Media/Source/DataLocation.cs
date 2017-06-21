using System;
using System.Threading.Tasks;

namespace Venz.Media
{
    public class DataLocation
    {
        public UInt32 Position { get; }
        public UInt32 Length { get; }

        public DataLocation(UInt32 position, UInt32 length)
        {
            Position = position;
            Length = length;
        }

        public static Task<Byte[]> GetDataAsync(IBinaryStream stream, DataLocation dataLocation)
        {
            stream.Position = dataLocation.Position;
            return stream.ReadAsync(dataLocation.Length);
        }
    }
}
