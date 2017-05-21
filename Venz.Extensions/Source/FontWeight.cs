using System;
using Windows.UI.Text;

namespace Venz.Extensions
{
    public static class FontWeightExtensions
    {
        public static String ToXamlValue(this FontWeight source)
        {
            if (source.Equals(FontWeights.Black))
                return "Black";
            else if (source.Equals(FontWeights.Bold))
                return "Bold";
            else if (source.Equals(FontWeights.ExtraBlack))
                return "ExtraBlack";
            else if (source.Equals(FontWeights.ExtraBold))
                return "ExtraBold";
            else if (source.Equals(FontWeights.ExtraLight))
                return "ExtraLight";
            else if (source.Equals(FontWeights.Light))
                return "Light";
            else if (source.Equals(FontWeights.Medium))
                return "Medium";
            else if (source.Equals(FontWeights.Normal))
                return "Normal";
            else if (source.Equals(FontWeights.SemiBold))
                return "SemiBold";
            else if (source.Equals(FontWeights.SemiLight))
                return "SemiLight";
            else if (source.Equals(FontWeights.Thin))
                return "Thin";
            else
                throw new ArgumentException("Can't convert FontWeight to XAML value because specified value is not supported by ToXamlValue method.");
        }
    }
}
