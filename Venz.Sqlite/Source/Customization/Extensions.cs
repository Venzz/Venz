using System;
using System.Text;

namespace Venz.Sqlite
{
    public static class Extensions
    {
        public static String ToSqlite(this String value) => value.Replace("'", "''");
        public static String ToSqlite(this DateTime value) => value.ToString("yyyy-MM-dd HH:mm:ss");
        public static Int32 ToSqlite(this Boolean value) => value ? 1 : 0;
        public static String ToSqlite(this Byte[] bytes)
        {
            var value = new StringBuilder("X'");
            foreach (var byteValue in bytes)
                value.Append(byteValue.ToString("X2"));
            value.Append("'");
            return value.ToString();
        }
    }
}
