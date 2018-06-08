using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public sealed partial class SplitViewMenuItem: UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(IconElement), typeof(SplitViewMenuItem),
            new PropertyMetadata(null, (obj, args) => ((SplitViewMenuItem)obj).OnIconChanged((IconElement)args.NewValue)));

        public IconElement Icon
        {
            get { return (IconElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(SplitViewMenuItem),
            new PropertyMetadata("", (obj, args) => ((SplitViewMenuItem)obj).OnTextChanged((String)args.NewValue)));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        

        public SplitViewMenuItem() { InitializeComponent(); }

        private void OnIconChanged(IconElement iconElement) => ContentControl.Content = iconElement;

        private void OnTextChanged(String text) => TextControl.Text = text;        
    }
}
