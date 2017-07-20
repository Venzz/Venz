using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public sealed partial class SplitViewMenuItem: UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(IconElement), typeof(SplitViewMenuItem), new PropertyMetadata(null));

        public IconElement Icon
        {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(SplitViewMenuItem), new PropertyMetadata(""));

        public String Text
        {
            get => (String)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        


        public SplitViewMenuItem()
        {
            InitializeComponent();
        }
    }
}
