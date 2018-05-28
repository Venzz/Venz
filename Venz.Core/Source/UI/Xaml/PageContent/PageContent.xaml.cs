using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml
{
    public class PageContent: ContentControl
    {
        private Grid CustomContentLayerControl;

        public PageContent()
        {
            DefaultStyleKey = typeof(PageContent);
        }

        public Grid GetCustomContentLayer() => CustomContentLayerControl;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CustomContentLayerControl = (Grid)GetTemplateChild(nameof(CustomContentLayerControl));
        }
    }
}
