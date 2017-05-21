using System;
using Windows.UI;

namespace Venz.Extensions
{
    public static class ColorExtensions
    {
        public static String ToXamlValue(this Color source) => $"#{source.A:X2}{source.R:X2}{source.G:X2}{source.B:X2}";
    }
}
