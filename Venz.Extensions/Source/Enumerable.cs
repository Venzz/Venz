using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Venz.Extensions
{
    public static class Enumerable
    {
        public static UInt32? IndexOf<TSource>(this IEnumerable<TSource> collection, Func<TSource, Boolean> action)
        {
            UInt32? index = 0;
            foreach (var item in collection)
            {
                if (action.Invoke(item))
                    return index;
                index++;
            }
            return null;
        }

        public static Boolean ContainsAtLeast<TSource>(this IEnumerable<TSource> collection, UInt32 amount, Func<TSource, Boolean> action)
        {
            var found = 0;
            foreach (var item in collection)
            {
                if (action.Invoke(item))
                    found++;
                if (found == amount)
                    return true;
            }
            return false;
        }

        public static Boolean IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (source is ICollection<TSource>)
                return ((ICollection<TSource>)source).Count == 0;
            if (source is ICollection)
                return ((ICollection)source).Count == 0;
            if (source is IReadOnlyCollection<TSource>)
                return ((IReadOnlyCollection<TSource>)source).Count == 0;
            foreach (var item in source)
                return false;
            return true;
        }
    }
}
