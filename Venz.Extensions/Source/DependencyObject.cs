using System;
using System.Collections.Generic;
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

        public static IReadOnlyCollection<T> TryFindVisualTreeChildren<T>(this DependencyObject source) where T: DependencyObject
        {
            return TryFindVisualTreeChildren<T>(source, null, null);
        }

        public static IReadOnlyCollection<T> TryFindVisualTreeChildren<T>(this DependencyObject source, DependencyProperty property, Object value) where T: DependencyObject
        {
            var children = new List<T>();
            if (source == null)
                return children;

            var childCount = VisualTreeHelper.GetChildrenCount(source);
            for (var i = 0; i < childCount; i++)
            {
                var sourceChild = VisualTreeHelper.GetChild(source, i);
                if ((sourceChild is T) && ((property == null) || Object.Equals(((T)sourceChild).GetValue(property), value)))
                {
                    children.Add((T)sourceChild);
                }
                else
                {
                    if ((sourceChild is Windows.UI.Xaml.Controls.ContentPresenter) && (((Windows.UI.Xaml.Controls.ContentPresenter)sourceChild).Content is DependencyObject))
                    {
                        sourceChild = (DependencyObject)((Windows.UI.Xaml.Controls.ContentPresenter)sourceChild).Content;
                        if (sourceChild is T)
                            children.Add((T)sourceChild);
                    }
                    children.AddRange(TryFindVisualTreeChildren<T>(sourceChild, property, value));
                }
            }
            return children;
        }
    }
}
