using System;

namespace Venz.Extensions
{
    public static class DateTimeExtensions
    {
        public static Int32 ToUnixTimestamp(this DateTime source)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dateTime = (source == default(DateTime)) ? epoch : source;
            return (Int32)(dateTime - epoch).TotalSeconds;
        }

        public static UInt64 ToUnixTimestampNano(this DateTime source)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (UInt64)(source - epoch).Ticks * 100;
        }

        public static DateTime FromUnixTimestamp(Int32 unixTimestamp)
        {
            if (unixTimestamp < 0)
                return default(DateTime);

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimestamp);
        }
    }
}
