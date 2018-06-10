using System;
using Windows.UI.Xaml;

namespace Venz.UI.Xaml.Controls
{
    public static class PasswordBox
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(String), typeof(PasswordBox),
            new PropertyMetadata(Byte.MaxValue, (obj, args) => OnPasswordChanged((global::Windows.UI.Xaml.Controls.PasswordBox)obj, args.OldValue, (String)args.NewValue)));

        public static String GetPassword(DependencyObject obj) => (String)obj.GetValue(PasswordProperty);

        public static void SetPassword(DependencyObject obj, String value) => obj.SetValue(PasswordProperty, value);

        private static void OnPasswordChanged(global::Windows.UI.Xaml.Controls.PasswordBox element, Object oldPassword, String password)
        {
            element.Unloaded -= OnElementUnloaded;
            element.Unloaded += OnElementUnloaded;
            element.PasswordChanged -= OnPasswordBoxPasswordChanged;
            if (!(oldPassword is Byte))
                element.Password = password ?? "";
            element.PasswordChanged += OnPasswordBoxPasswordChanged;
        }

        private static void OnPasswordBoxPasswordChanged(Object sender, RoutedEventArgs args)
        {
            var passwordBox = (global::Windows.UI.Xaml.Controls.PasswordBox)sender;
            SetPassword(passwordBox, passwordBox.Password);
        }

        private static void OnElementUnloaded(Object sender, RoutedEventArgs args)
        {
            var passwordBox = (global::Windows.UI.Xaml.Controls.PasswordBox)sender;
            passwordBox.PasswordChanged -= OnPasswordBoxPasswordChanged;
        }
    }
}