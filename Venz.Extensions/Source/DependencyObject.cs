using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Venz.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T TryGetVisualTreeChildAt<T>(this DependencyObject source, UInt32 level, UInt32 position = 0) where T: DependencyObject
        {
            var i = 0;
            var child = source;
            while (i++ < level)
            {
                if (VisualTreeHelper.GetChildrenCount(child) == 0)
                    return null;

                child = VisualTreeHelper.GetChild(child, 0);
            }

            if (VisualTreeHelper.GetChildrenCount(child) <= position)
                return null;

            child = VisualTreeHelper.GetChild(child, (Int32)position);
            return (child is T) ? (T)child : null;
        }
    }
}
