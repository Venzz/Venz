using System;
using Windows.Foundation.Metadata;

namespace Venz.Windows
{
    public static class PlatformVersion
    {
        public static Boolean Windows10Build14393 => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3, 0);
    }
}
