using System;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Security.ExchangeActiveSyncProvisioning;

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
        public static String ApplicationPlatform { get; } = "Windows.Mobile.81";
        public static DeviceFamily DeviceFamily { get; }
        public static String DeviceModel { get; }
        public static String DeviceManufacturer { get; }
        public static DeviceType DeviceType { get; }
        public static Boolean HasBuild14393Api => false;

        static SystemInfo()
        {
            OsVersion = "8.1";
            DeviceFamily = DeviceFamily.Mobile;

            var package = Package.Current;
            ApplicationPackageName = "Property isn't supported.";
            ApplicationPackageFamilyName = package.Id.FamilyName;
            ApplicationPackageVersion = new Version(package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision);

            var clientDeviceInformation = new EasClientDeviceInformation();
            DeviceManufacturer = clientDeviceInformation.SystemManufacturer;
            DeviceModel = clientDeviceInformation.SystemProductName;
            if (DeviceModel == "Virtual")
            {
                DeviceType = DeviceType.Emulator;
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
