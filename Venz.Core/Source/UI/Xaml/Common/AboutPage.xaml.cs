using System;
using System.Collections.Generic;
using Venz.Core;
using Windows.ApplicationModel.Email;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Venz.UI.Xaml.Common
{
    public sealed partial class AboutPage: Page
    {
        public static UIElement ExtendedContent { get; set; }

        private String ApplicationTitle;



        public AboutPage()
        {
            InitializeComponent();
            DataContext = new Context();
            ExtendedContentControl.Content = AboutPage.ExtendedContent;
        }
        
        protected override void SetState(FrameNavigation.Parameter navigationParameter, FrameNavigation.Parameter stateParameter)
        {
            ApplicationTitle = $"{navigationParameter.Get("title")} ({navigationParameter.Get("version")}), OS: {SystemInfo.OsVersion}";
        }

        private async void OnAllAppsButtonClicked(Object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store:search?publisher=Venz", UriKind.Absolute));
        }

        private async void OnSendEmailButtonClicked(Object sender, RoutedEventArgs e)
        {
            var email = new EmailMessage();
            email.Subject = ApplicationTitle;
            email.To.Add(new EmailRecipient("venz.wp.development@gmail.com", "Venz"));
            await EmailManager.ShowComposeNewEmailAsync(email).AsTask().ConfigureAwait(false);
        }

        private async void Application_Tapped(Object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var app = ((FrameworkElement)sender).DataContext as Application;
            await Launcher.LaunchUriAsync(new Uri(app.Url, UriKind.Absolute));
        }

        public static FrameNavigation.Parameter GetNavigationParameter(String applicationTitle)
        {
            var parameter = new FrameNavigation.Parameter("title", applicationTitle);
            parameter.Add("version", SystemInfo.ApplicationPackageVersion.ToString());
            return parameter;
        }

        private class Context
        {
            public List<Application> Apps { get; private set; }

            public Context()
            {
                Apps = new List<Application>()
                {
                    new Application()
                    {
                        Title = "Ringtone Maker",
                        IconBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2)),
                        Icon = new BitmapImage(new Uri("ms-appx:///Resources/Misc/RingtoneMaker.png", UriKind.Absolute)),
                        Description = "Make a ringtone with mp3 from your phone",
                        Url = "ms-windows-store:navigate?appid=63cb6ea0-c92c-4de6-8fca-3fcb95f45f06"
                    },
                    new Application()
                    {
                        Title = "Start Screen Clock",
                        IconBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0xA9, 0x17)),
                        Icon = new BitmapImage(new Uri("ms-appx:///Resources/Misc/StartScreenClock.png", UriKind.Absolute)),
                        Description = "Customizable clock that updates every minute",
                        Url = "ms-windows-store:navigate?appid=f412b9a4-28ad-4b29-a525-93c544accfad"
                    }
                };
            }
        }

        private class Application
        {
            public String Title { get; set; }
            public Brush IconBackground { get; set; }
            public ImageSource Icon { get; set; }
            public String Description { get; set; }
            public String Url { get; set; }
        }
    }
}
