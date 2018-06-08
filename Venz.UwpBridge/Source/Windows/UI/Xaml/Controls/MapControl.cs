using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Maps;

namespace Windows.UI.Xaml.Controls
{
    public static class MapControlExtensions
    {
        public static IAsyncOperation<Boolean> TryZoomInAsync(this MapControl mapControl)
        {
            return mapControl.TrySetViewAsync(mapControl.Center, mapControl.ZoomLevel + 1);
        }

        public static IAsyncOperation<Boolean> TryZoomOutAsync(this MapControl mapControl)
        {
            return mapControl.TrySetViewAsync(mapControl.Center, mapControl.ZoomLevel - 1);
        }
    }
}
