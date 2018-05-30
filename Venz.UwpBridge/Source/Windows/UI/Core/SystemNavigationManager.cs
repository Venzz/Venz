using System;
using Windows.Phone.UI.Input;

namespace Windows.UI.Core
{
    public class SystemNavigationManager
    {
        public AppViewBackButtonVisibility AppViewBackButtonVisibility { get; set; }

        public event EventHandler<BackRequestedEventArgs> BackRequested;



        private SystemNavigationManager()
        {
            HardwareButtons.BackPressed += OnBackPressed;
        }

        private void OnBackPressed(Object sender, BackPressedEventArgs args)
        {
            if (BackRequested != null)
            {
                var backRequestedEventArgs = new BackRequestedEventArgs();
                BackRequested(this, backRequestedEventArgs);
                args.Handled = backRequestedEventArgs.Handled;
            }
        }

        public static SystemNavigationManager GetForCurrentView() => new SystemNavigationManager();
    }
}
