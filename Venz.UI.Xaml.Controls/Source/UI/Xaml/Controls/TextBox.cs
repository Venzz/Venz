using System;
using Windows.UI.Xaml;

namespace Venz.UI.Xaml.Controls
{
    public class TextBox: global::Windows.UI.Xaml.Controls.TextBox
    {
        public static readonly DependencyProperty UpdatableTextProperty =
            DependencyProperty.Register("UpdatableText", typeof(String), typeof(TextBox),
            new PropertyMetadata("", (obj, args) => ((TextBox)obj).OnUpdatableTextChanged((String)args.OldValue, (String)args.NewValue)));

        public String UpdatableText
        {
            get => (String)GetValue(UpdatableTextProperty);
            set => SetValue(UpdatableTextProperty, value);
        }



        public TextBox()
        {
            TextChanged += (s, e) => OnTextChanged(Text);
        }

        private void OnUpdatableTextChanged(String oldValue, String newValue) => Text = newValue;

        protected virtual void OnTextChanged(String value) => UpdatableText = value;
    }
}
