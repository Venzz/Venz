using System;

namespace Venz.Extensions
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Return a value in the (10 min 15.5 sec) format.
        /// </summary>
        public static String ToString(this TimeSpan source, String minString, String secString)
        {
            String minutes = (source.TotalMinutes != 0) ? $"{(Int32)source.TotalMinutes} {minString} " : "";
            if ((source.Seconds != 0) || (source.Milliseconds != 0))
            {
                String seconds = source.Seconds.ToString();
                String mseconds = (source.Milliseconds > 0) ? "." + source.Milliseconds.ToString().Substring(0, 1) : "";
                return $"{minutes}{seconds}{mseconds} {secString}";
            }

            if (source.TotalSeconds == 0)
                return $"0 {secString}";

            return minutes;
        }

        /// <summary>
        /// Return a value in the hh:mm:ss format.
        /// </summary>
        public static String ToColonFormat(this TimeSpan source) => source.ToString(@"mm\:ss");
    }
}
