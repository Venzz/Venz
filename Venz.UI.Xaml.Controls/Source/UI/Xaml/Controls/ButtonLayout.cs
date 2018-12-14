using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public class ButtonLayout: Panel
    {
        public static readonly DependencyProperty ButtonMarginProperty =
            DependencyProperty.Register("ButtonMargin", typeof(Double), typeof(ButtonLayout),
            new PropertyMetadata(12.0, (obj, args) => ((ButtonLayout)obj).InvalidateMeasure()));

        public Double ButtonMargin
        {
            get => (Double)GetValue(ButtonMarginProperty);
            set => SetValue(ButtonMarginProperty, value);
        }

        public static readonly DependencyProperty MaxButtonWidthProperty =
            DependencyProperty.Register("MaxButtonWidth", typeof(Double), typeof(ButtonLayout),
            new PropertyMetadata(Double.NaN, (obj, args) => ((ButtonLayout)obj).InvalidateMeasure()));

        public Double MaxButtonWidth
        {
            get => (Double)GetValue(MaxButtonWidthProperty);
            set => SetValue(MaxButtonWidthProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(Int32), typeof(ButtonLayout),
            new PropertyMetadata(0, (obj, args) => ((ButtonLayout)obj).InvalidateMeasure()));

        public Int32 Columns
        {
            get => (Int32)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }



        protected override Size ArrangeOverride(Size finalSize)
        {
            var buttonWidth = GetButtonWidth(finalSize);
            for (var i = 0; i < Children.Count; i++)
            {
                var position = new Point((ButtonMargin + buttonWidth) * i, 0);
                Children[i].Arrange(new Rect(position, new Size(buttonWidth, Children[i].DesiredSize.Height)));
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Double.IsInfinity(availableSize.Width))
            {
                return base.MeasureOverride(availableSize);
            }
            else
            {
                var buttonSize = new Size(GetButtonWidth(availableSize), availableSize.Height);
                foreach (var child in Children)
                    child.Measure(buttonSize);
                return new Size(availableSize.Width, Children.Max(a => a.DesiredSize.Height));
            }
        }

        private Double GetButtonWidth(Size size)
        {
            var columns = (Columns > 0) ? Columns : Children.Count;
            var buttonWidth = (size.Width - (columns - 1) * ButtonMargin) / columns;
            if (!Double.IsNaN(MaxButtonWidth) && (buttonWidth > MaxButtonWidth))
                buttonWidth = MaxButtonWidth;
            if (buttonWidth < 0)
                buttonWidth = 0;
            return buttonWidth;
        }
    }
}
