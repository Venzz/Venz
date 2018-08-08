using System;
using Windows.UI.Xaml;

namespace Venz.UI.Xaml
{
    public class Flyout: global::Windows.UI.Xaml.Controls.Flyout
    {
        public Object DataContext { set => ((FrameworkElement)Content).DataContext = value; }

        public Flyout()
        {
            Opened += (sender, args) => SetState();
        }

        protected virtual void SetState() { }
    }
}
