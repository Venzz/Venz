using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Venz.Extensions
{
    public static class ScrollViewerExtensions
    {
        public static void HintVerticalScrollBar(this ScrollViewer source)
        {
            var scrollViewerContents = source.TryGetVisualTreeChildAt<Grid>(level: 2);
            if ((scrollViewerContents == null) || (scrollViewerContents.Children.Count < 1))
                return;
            var scrollBar = scrollViewerContents.Children[1] as ScrollBar;
            if (scrollBar == null)
                return;

            var currentContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            if (currentContextScheduler == null)
                return;

            var prevIndicatorMode = scrollBar.IndicatorMode;
            var showIndicatorTask = Task.Delay(250).ContinueWith((prevTask) => scrollBar.IndicatorMode = ScrollingIndicatorMode.TouchIndicator, currentContextScheduler);
            showIndicatorTask.ContinueWith((prevTask) => Task.Delay(500).ContinueWith((delayTask) => scrollBar.IndicatorMode = prevIndicatorMode, currentContextScheduler));
        }
    }
}
