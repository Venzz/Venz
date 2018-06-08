using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Venz.UI.Xaml.Controls
{
    public class Button: global::Windows.UI.Xaml.Controls.Button
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(String), typeof(Button),
            new PropertyMetadata(null, (obj, args) => ((Button)obj).OnLabelChanged((String)args.NewValue)));

        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty AvailableWidthProperty =
            DependencyProperty.Register("AvailableWidth", typeof(Double), typeof(Button), new PropertyMetadata(720.0));

        public Double AvailableWidth
        {
            get { return (Double)GetValue(AvailableWidthProperty); }
            set { SetValue(AvailableWidthProperty, value); }
        }



        protected override Size MeasureOverride(Size availableSize)
        {
            if (Double.IsInfinity(availableSize.Width))
            {
                return base.MeasureOverride(availableSize);
            }
            else
            {
                var defaultSize = base.MeasureOverride(availableSize);
                if (availableSize.Width == 0)
                    return defaultSize;

                return new Size((availableSize.Width < AvailableWidth) ? (availableSize.Width / 2 - 12) : AvailableWidth / 2, defaultSize.Height);
            }
        }

        private void OnLabelChanged(String value) => Content = value;
    }
}
