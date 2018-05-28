using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml
{
    public class PageContent: ContentControl
    {
        private Grid NotificationsLayerControl;
        private Grid CustomContentLayerControl;

        public static readonly DependencyProperty NotificationsLayerContentProperty =
            DependencyProperty.Register(nameof(NotificationsLayerContent), typeof(UIElement), typeof(PageContent),
            new PropertyMetadata(null, (obj, args) => ((PageContent)obj).OnNotificationsLayerContentChanged((UIElement)args.NewValue)));

        public UIElement NotificationsLayerContent
        {
            get => (UIElement)GetValue(NotificationsLayerContentProperty);
            set => SetValue(NotificationsLayerContentProperty, value);
        }



        public PageContent()
        {
            DefaultStyleKey = typeof(PageContent);
        }

        public Grid GetCustomContentLayer() => CustomContentLayerControl;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            NotificationsLayerControl = (Grid)GetTemplateChild(nameof(NotificationsLayerControl));
            CustomContentLayerControl = (Grid)GetTemplateChild(nameof(CustomContentLayerControl));
            if ((NotificationsLayerControl != null) && (NotificationsLayerContent != null))
                NotificationsLayerControl.Children.Add(NotificationsLayerContent);
        }

        private void OnNotificationsLayerContentChanged(UIElement value)
        {
            if (NotificationsLayerControl == null)
                return;

            if (NotificationsLayerControl.Children.Count > 1)
                NotificationsLayerControl.Children.RemoveAt(1);
            if (value != null)
                NotificationsLayerControl.Children.Add(value);
        }
    }
}
