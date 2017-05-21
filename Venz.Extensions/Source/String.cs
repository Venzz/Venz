using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Venz.Extensions
{
    public static class StringExtensions
    {
        public static String Between(this String source, String leftValue, Boolean allowFromBeginning, String rightValue, Boolean allowToEnd)
        {
            var index1 = (leftValue == null) ? -1 : source.IndexOf(leftValue);
            if (index1 == -1)
            {
                if (allowFromBeginning)
                    index1 = 0;
                else
                    return null;
            }
            else
            {
                index1 += leftValue.Length;
            }

            var index2 = (rightValue == null) ? -1 : source.IndexOf(rightValue, index1);
            if (index2 == -1)
            {
                if (allowToEnd)
                    index2 = source.Length;
                else
                    return null;
            }

            return source.Substring(index1, index2 - index1);
        }

        public static String Between(this String source, String leftValue, String rightValue) => source.Between(leftValue, false, rightValue, false);

        public static Boolean IsMatchedMinimumLength(String source, Int32 minimumLength)
        {
            if ((source == null) || String.IsNullOrWhiteSpace(source))
                return false;
            return source.Length >= minimumLength;
        }
        
        public static String Remove(this String source, IEnumerable<Char> characters)
        {
            if ((source == null) || (source.Length == 0))
                return source;

            var modifiedString = "";
            foreach (var character in source)
                if (!characters.Contains(character))
                    modifiedString += character;
            return modifiedString;
        }

        public static String RemoveInvalidFileNameChars(this String source)
        {
            var invalidCharacters = Path.GetInvalidFileNameChars();
            var parts = source.Split(invalidCharacters, StringSplitOptions.RemoveEmptyEntries);
            return String.Concat(parts);
        }

        public static Boolean ContainsInvalidFileNameChars(this String source)
        {
            var invalidCharacters = Path.GetInvalidFileNameChars();
            var parts = source.Split(invalidCharacters, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length != 1;
        }

        public static String TruncateTo(this String source, Int32 maxLength) => (source.Length <= maxLength) ? source : source.Remove(maxLength);
    }
}
