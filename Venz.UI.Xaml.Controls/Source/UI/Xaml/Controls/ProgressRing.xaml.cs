using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public sealed partial class ProgressRing: UserControl
    {
        private global::Windows.UI.Xaml.Controls.ProgressRing ProgressRingControl => (global::Windows.UI.Xaml.Controls.ProgressRing)((Grid)Content).Children[0];
        private TextBlock LabelControl => (TextBlock)((Grid)Content).Children[1];

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(String), typeof(ProgressRing),
            new PropertyMetadata("", (obj, args) => ((ProgressRing)obj).OnLabelChanged((String)args.NewValue)));

        public String Label
        {
            get => (String)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(ProgressRingSize), typeof(ProgressRing),
            new PropertyMetadata(ProgressRingSize.Small, (obj, args) => ((ProgressRing)obj).OnSizeChanged((ProgressRingSize)args.NewValue)));

        public ProgressRingSize Size
        {
            get => (ProgressRingSize)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }



        public ProgressRing()
        {
            InitializeComponent();
            UniversalDependencyObject.RegisterPropertyChangedCallback(this, VisibilityProperty, nameof(Visibility), (sender, args) => OnVisibilityChanged(Visibility));
        }

        private void OnLabelChanged(String value) => Apply(value, Size);

        private void OnSizeChanged(ProgressRingSize value) => Apply(Label, value);

        private void OnVisibilityChanged(Visibility value) => ProgressRingControl.IsActive = (value == Visibility.Visible);

        private void Apply(String label, ProgressRingSize size)
        {
            if (String.IsNullOrWhiteSpace(label))
            {
                ProgressRingControl.Margin = new Thickness();
                LabelControl.Text = "";
            }
            else
            {
                ProgressRingControl.Margin = GetMargin(size);
                ProgressRingControl.Width = GetSize(size);
                ProgressRingControl.Height = GetSize(size);
                LabelControl.Text = label;
                LabelControl.FontSize = GetFontSize(size);
            }
        }



        private static Thickness GetMargin(ProgressRingSize size)
        {
            switch (size)
            {
                case ProgressRingSize.Small:
                    return new Thickness(0, 0, 5, 0);
                case ProgressRingSize.Medium:
                    return new Thickness(0, 0, 10, 0);
                case ProgressRingSize.Large:
                    return new Thickness(0, 0, 20, 0);
                default:
                    throw new NotSupportedException();
            }
        }

        private static Double GetFontSize(ProgressRingSize size)
        {
            switch (size)
            {
                case ProgressRingSize.Small:
                    return 11;
                case ProgressRingSize.Medium:
                    return 20;
                case ProgressRingSize.Large:
                    return 42;
                default:
                    throw new NotSupportedException();
            }
        }

        private static Double GetSize(ProgressRingSize size)
        {
            switch (size)
            {
                case ProgressRingSize.Small:
                    return 20;
                case ProgressRingSize.Medium:
                    return 40;
                case ProgressRingSize.Large:
                    return 60;
                default:
                    throw new NotSupportedException();
            }
        }

        public enum ProgressRingSize { Small, Medium, Large }
    }
}
