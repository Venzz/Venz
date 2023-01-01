using System;
using System.Threading.Tasks;

namespace Venz.Media
{
    public interface IBinaryStream: IDisposable
    {
        UInt32 Position { get; set; }
        Task<UInt32> GetLengthAsync();
        Task<Byte[]> ReadAsync(UInt32 amount);
    }
}
