using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Venz.Extensions
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Creates transform object that can be used to translate current element to absolute position and size of target element.
        /// </summary>
        public static CompositeTransform GetTransformTo(this FrameworkElement source, FrameworkElement target)
        {
            var transform = new CompositeTransform();
            var sourceAbsolutePosition = source.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
            var targetAbsolutePosition = target.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
            transform.TranslateX = targetAbsolutePosition.X - sourceAbsolutePosition.X;
            transform.TranslateY = targetAbsolutePosition.Y - sourceAbsolutePosition.Y;
            transform.ScaleX = target.ActualWidth / source.ActualWidth;
            transform.ScaleY = target.ActualHeight / source.ActualHeight;
            return transform;
        }

        public static List<T> GetHitTestVisibleOnScreenItems<T>(this FrameworkElement source) where T: UIElement
        {
            return GetHitTestVisibleOnScreenItems<T>(source, source.ActualWidth, source.ActualHeight);
        }

        public static List<T> GetHitTestVisibleOnScreenItems<T>(this FrameworkElement source, Double width, Double height) where T: UIElement
        {
            return GetOnScreenItems<T>(source, new Point(0, 0), width, height, true);
        }

        public static List<T> GetOnScreenItems<T>(this FrameworkElement source) where T: UIElement
        {
            if ((source.ActualWidth == 0) || (source.ActualHeight == 0))
                return new List<T>();
            return GetOnScreenItems<T>(source, source.ActualWidth, source.ActualHeight);
        }

        public static List<T> GetOnScreenItems<T>(this FrameworkElement source, Double width, Double height) where T: UIElement
        {
            return GetOnScreenItems<T>(source, new Point(0, 0), width, height, false);
        }

        public static List<T> GetOnScreenItems<T>(this FrameworkElement source, Point point, Double width, Double height) where T: UIElement
        {
            return GetOnScreenItems<T>(source, point, width, height, false);
        }

        private static List<T> GetOnScreenItems<T>(this FrameworkElement source, Point point, Double width, Double height, Boolean isHitTestVisible) where T: UIElement
        {
            if (Window.Current.Content == null)
                throw new ArgumentNullException("GetOnScreenItems: Window.Current.Content");

            var absoluteScreenPosition = source.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
            var onScreenItems = new List<T>();
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(new Rect(absoluteScreenPosition.X + point.X, absoluteScreenPosition.Y + point.Y, width, height), source, !isHitTestVisible);
            foreach (var element in elements)
                if (element is T)
                    onScreenItems.Add(element as T);
            return onScreenItems;
        }
    }
}
