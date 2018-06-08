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
            if (level == 0)
                return source as T;

            var i = 0;
            var previousLevel = source;
            while (i++ < level - 1)
            {
                if (VisualTreeHelper.GetChildrenCount(previousLevel) == 0)
                    return null;
                previousLevel = VisualTreeHelper.GetChild(previousLevel, 0);
            }

            if (VisualTreeHelper.GetChildrenCount(previousLevel) <= position)
                return null;

            var child = VisualTreeHelper.GetChild(previousLevel, (Int32)position);
            return child as T;
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

        public static String PrintVisualTree(this DependencyObject source, Int32 level = 0)
        {
            var sourceElement = source;
            if ((sourceElement is Windows.UI.Xaml.Controls.ContentPresenter) && (((Windows.UI.Xaml.Controls.ContentPresenter)sourceElement).Content is DependencyObject))
                sourceElement = (DependencyObject)((Windows.UI.Xaml.Controls.ContentPresenter)sourceElement).Content;
            if (sourceElement == null)
                return null;

            var indent = "";
            for (var i = 0; i < level; i++)
                indent += "    ";

            var visualTree = "";
            var childCount = VisualTreeHelper.GetChildrenCount(sourceElement);
            for (var i = 0; i < childCount; i++)
            {
                var sourceChild = VisualTreeHelper.GetChild(sourceElement, i);
                visualTree += $"{indent}{sourceChild.GetType().FullName}\n";
                visualTree += PrintVisualTree(sourceChild, level + 1);
            }
            return visualTree;
        }
    }
}
