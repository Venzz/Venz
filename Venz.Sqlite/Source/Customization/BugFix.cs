using System;

namespace SQLite
{
    internal static class BugFix
    {
        /// <summary>
        /// SQLite v3.8.10.2
        /// During INSERT operation DateTime field that is converted using this code ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") in the method BindParameter
        /// will be returned in the method ReadCol as 2015-10-24T10:50:34Z. DateTime.Parse will correctly convert this to UTC.
        /// But, during UPDATE operation the same code will store DateTime as string and now this value will be returned as 2015-10-24 10:50:34
        /// and of course DateTime.Parse will treat this value as a Local by default.
        /// </summary>

        /// <summary>
        /// Looks like previous description was wrong. During INSERT operation date time is stored and restored as "yyyy-MM-dd HH:mm:ss".
        /// During UPDATE operation DateTime gets manually converted to "yyyy-MM-dd HH:mm:ss", so all looks good.
        /// </summary>
        public static DateTime DateColumnTextToDateTime(String dateAsText) => DateTime.Parse(dateAsText, null, System.Globalization.DateTimeStyles.AssumeUniversal);
    }
}
