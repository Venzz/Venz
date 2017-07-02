using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Venz.UI.Xaml
{
    public sealed partial class PageContent: UserControl
    {
        public PageContent()
        {
            InitializeComponent();
            CustomContentLayerControl.SetBinding(Panel.BackgroundProperty, new Binding() { Path = new PropertyPath(nameof(Background)), Source = this });
        }

        public Grid GetCustomContentLayer() => CustomContentLayerControl;
    }
}
