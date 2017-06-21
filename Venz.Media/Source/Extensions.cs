using System;

namespace Venz.Media
{
    internal static class Extensions
    {
        public static UInt32? IndexOf(this Byte[] array, String sequence)
        {
            for (var i = 0U; i < array.Length; i++)
            {
                var index = 0U;
                var matches = 0;
                foreach (var sequenceValue in sequence)
                {
                    if (array[i + index] == sequenceValue)
                        matches++;
                    else
                        break;

                    index++;
                }
                if (sequence.Length == matches)
                {
                    return i;
                }

                index++;
            }

            return null;
        }
    }
}
