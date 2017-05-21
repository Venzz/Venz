using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.System;

namespace Venz.Windows
{
    public static class ServiceLauncher
    {
        public static Task<Boolean> ShowReviewRequestAsync(String appId) => Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={appId}", UriKind.Absolute)).AsTask();

        public static Task ShowLocationAsync(Double latitude, Double longitude, Byte zoom)
        {
            return Launcher.LaunchUriAsync(new Uri($"bingmaps:?cp={latitude.ToString(CultureInfo.InvariantCulture)}~{longitude.ToString(CultureInfo.InvariantCulture)}&lvl={zoom}", UriKind.Absolute)).AsTask();
        }

        public static class FeedbackHub
        {
            public static Boolean IsAvailable => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3, 0);
            public static Task OpenAsync(String applicationPackageFamilyName) => Launcher.LaunchUriAsync(new Uri($"windows-feedback:///?appid={applicationPackageFamilyName}!App", UriKind.Absolute)).AsTask();
        }

        public static class Settings
        {
            public static Task<Boolean> OpenLocationAsync() => Launcher.LaunchUriAsync(new Uri("ms-settings-location:", UriKind.Absolute)).AsTask();
            public static Task<Boolean> OpenBackgroundAppsAsync() => Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-backgroundapps", UriKind.Absolute)).AsTask();
            public static Task<Boolean> OpenBatteryUsageByAppAsync() => Launcher.LaunchUriAsync(new Uri("ms-settings:batterysaver-usagedetails", UriKind.Absolute)).AsTask();
        }
    }
}
