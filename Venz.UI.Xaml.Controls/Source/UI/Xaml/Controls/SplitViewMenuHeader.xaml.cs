using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public sealed partial class SplitViewMenuHeader: UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(SplitViewMenuHeader),
            new PropertyMetadata("", (obj, args) => ((SplitViewMenuHeader)obj).OnTextChanged((String)args.NewValue)));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        


        public SplitViewMenuHeader()
        {
            InitializeComponent();
        }

        private void OnTextChanged(String value) => TextControl.Text = value.ToUpper();
    }
}
