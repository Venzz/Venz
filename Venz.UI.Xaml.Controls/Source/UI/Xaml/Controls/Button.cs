using System;
using Windows.Foundation;
using Windows.UI.Core;
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
            get => (String)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register("Cursor", typeof(CoreCursorType), typeof(Button), new PropertyMetadata(CoreCursorType.Arrow));

        public CoreCursorType Cursor
        {
            get => (CoreCursorType)GetValue(CursorProperty);
            set => SetValue(CursorProperty, value);
        }



        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("RootGrid") is global::Windows.UI.Xaml.Controls.Grid root)
            {
                var visualStateGroups = VisualStateManager.GetVisualStateGroups(root);
                if (visualStateGroups.Count > 0)
                    visualStateGroups[0].CurrentStateChanged += OnVisualStateChanged;
            }
        }

        private void OnVisualStateChanged(Object sender, VisualStateChangedEventArgs args)
        {
            if ((args.NewState?.Name == "PointerOver") && (Cursor != CoreCursorType.Arrow))
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(Cursor, 1);
        }

        private void OnLabelChanged(String value) => Content = value;
    }
}
