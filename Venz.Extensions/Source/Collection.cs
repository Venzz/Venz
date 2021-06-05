using System;
using System.Collections;
using System.Collections.Generic;

namespace Venz.Extensions
{
    public static class Collection
    {
        public static UInt32? IndexOf<TSource>(this IEnumerable<TSource> collection, Func<TSource, Boolean> filter)
        {
            UInt32? index = 0;
            foreach (var item in collection)
            {
                if (filter.Invoke(item))
                    return index;
                index++;
            }
            return null;
        }

        public static Boolean ContainsAtLeast<TSource>(this IEnumerable<TSource> collection, UInt32 amount, Func<TSource, Boolean> filter)
        {
            var found = 0;
            foreach (var item in collection)
            {
                if (filter.Invoke(item))
                    found++;
                if (found == amount)
                    return true;
            }
            return false;
        }

        public static Boolean IsEmpty<TSource>(this IEnumerable<TSource> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("source");
            if (collection is ICollection<TSource>)
                return ((ICollection<TSource>)collection).Count == 0;
            if (collection is ICollection)
                return ((ICollection)collection).Count == 0;
            if (collection is IReadOnlyCollection<TSource>)
                return ((IReadOnlyCollection<TSource>)collection).Count == 0;
            foreach (var item in collection)
                return false;
            return true;
        }

        public static Boolean Remove<TSource>(this ICollection<TSource> collection, Func<TSource, Boolean> filter)
        {
            Tuple<TSource> filteredItem = null;
            foreach (var item in collection)
            {
                if (filter.Invoke(item))
                {
                    filteredItem = new Tuple<TSource>(item);
                    break;
                }
            }

            if (filteredItem == null)
                return false;

            return collection.Remove(filteredItem.Item1);
        }
    }
}
