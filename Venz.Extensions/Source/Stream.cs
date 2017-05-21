using System;
using System.IO;

namespace Venz.Extensions
{
    public static class StreamExtensions
    {
        public static Byte[] GetBytes(this Stream sourse)
        {
            var bytes = new Byte[sourse.Length];
            sourse.Position = 0;
            sourse.Read(bytes, 0, (Int32)sourse.Length);
            return bytes;
        }
    }
}
