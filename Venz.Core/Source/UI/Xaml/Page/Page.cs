using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Venz.UI.Xaml
{
    public partial class Page: Windows.UI.Xaml.Controls.Page
    {
        /// <summary>
        /// Returns a value that shows if this page is the page that the user is currently interacting with.
        /// </summary>
        public Boolean IsActive => this == Active;

        /// <summary>
        /// Returns the page that the user is currently interacting with.
        /// </summary>
        public static Page Active => (Window.Current?.Content as Frame)?.Content as Page;



        public Page(): this(NavigationCacheMode.Disabled) { }

        public Page(NavigationCacheMode cacheMode)
        {
            if (DesignMode.DesignModeEnabled)
                RequestedTheme = ElementTheme.Light;

            Page_BackKey();
            Page_Navigation();

            IsTabStop = true;
            NavigationCacheMode = cacheMode;
            Loaded += OnLoaded;
        }

        protected virtual void OnLoaded(Object sender, RoutedEventArgs args)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (DataContext == null)
                throw new InvalidOperationException("DataContext is not set. DataContext is required for some features.");

            Navigation_OnNavigatedTo(args);
            BackKey_OnNavigatedTo(args);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            BackKey_OnNavigatingFrom(args);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);
            Navigation_OnNavigatedFrom(args);
        }
    }
}
