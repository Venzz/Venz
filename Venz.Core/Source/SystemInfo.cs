using System;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

#if BACKGROUND
namespace Venz.Background
#else
namespace Venz.Core
#endif
{
    public enum DeviceFamily { Desktop, Mobile, Unknown }

    public enum DeviceType { Real, Emulator }

    public enum DeviceScreenType { Phone, PhoneInContinuumMode, Tablet, Desktop }

    public static class SystemInfo
    {
        public static String OsVersion { get; }
        public static Version ApplicationPackageVersion { get; }
        public static String ApplicationPackageName { get; }
        public static String ApplicationPackageFamilyName { get; }
        public static String ApplicationPlatform { get; }
        public static DeviceFamily DeviceFamily { get; }
        public static String DeviceModel { get; }
        public static String DeviceManufacturer { get; }
        public static DeviceType DeviceType { get; }
        public static Boolean HasBuild14393Api => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3, 0);

        static SystemInfo()
        {
            var version = UInt64.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            OsVersion = $"{(version & 0xFFFF000000000000L) >> 48}.{(version & 0x0000FFFF00000000L) >> 32}.{(version & 0x00000000FFFF0000L) >> 16}.{(version & 0x000000000000FFFFL)}";
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop":
                    DeviceFamily = DeviceFamily.Desktop;
                    break;
                case "Windows.Mobile":
                    DeviceFamily = DeviceFamily.Mobile;
                    break;
                default:
                    DeviceFamily = DeviceFamily.Unknown;
                    break;
            }
            
            var package = Package.Current;
            ApplicationPackageName = package.DisplayName;
            ApplicationPackageFamilyName = package.Id.FamilyName;
            ApplicationPackageVersion = new Version(package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision);
            ApplicationPlatform = AnalyticsInfo.VersionInfo.DeviceFamily;

            var clientDeviceInformation = new EasClientDeviceInformation();
            DeviceManufacturer = clientDeviceInformation.SystemManufacturer;
            DeviceModel = clientDeviceInformation.SystemProductName;
            if (DeviceModel == "Virtual")
            {
                DeviceType = DeviceType.Emulator;
            }
            else if (DeviceFamily == DeviceFamily.Desktop)
            {
                if (DeviceManufacturer == "System manufacturer")
                    DeviceManufacturer = "PC";
                if (DeviceModel == "System Product Name")
                    DeviceModel = "PC";
            }
            if (String.IsNullOrWhiteSpace(DeviceManufacturer))
            {
                DeviceManufacturer = "Uspecified Manufacturer";
            }
            if (String.IsNullOrWhiteSpace(DeviceModel))
            {
                DeviceModel = "Uspecified Model";
            }
        }

        public static DeviceScreenType GetDeviceScreenType(Size availableSize)
        {
            if ((DeviceFamily == DeviceFamily.Mobile) && ((availableSize.Width < 450) && (availableSize.Height < 800) || (availableSize.Width < 800) && (availableSize.Height < 450)))
                return DeviceScreenType.Phone;
            return DeviceScreenType.Desktop;
        }
    }
}
