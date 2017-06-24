using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;

#if BACKGROUND
namespace Venz.Background
#else
namespace Venz.UI.Xaml
#endif
{
    public class ApplicationSettings
    {
        private IPropertySet Properties = ApplicationData.Current.LocalSettings.Values;



        protected T Get<T>(String propertyName) => Get<T>(propertyName, default(T));

        protected T Get<T>(String propertyName, T defaultValue)
        {
            if (Properties.ContainsKey(propertyName))
                return (T)Properties[propertyName];
            return defaultValue;
        }

        protected DateTime Get(String propertyName, DateTime defaultValue)
        {
            if (Properties.ContainsKey(propertyName))
                return new DateTime((Int64)Properties[propertyName]);
            return defaultValue;
        }

        protected void Set(String propertyName, Object value)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, value);
            else
                Properties[propertyName] = value;
        }

        protected void Set(String propertyName, DateTime value)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, value.Ticks);
            else
                Properties[propertyName] = value.Ticks;
        }

        //
        // Specialized
        //

        public Color? Get(String propertyName, Color? defaultValue)
        {
            if (Properties.ContainsKey(propertyName + "_a") && Properties.ContainsKey(propertyName + "_r") && Properties.ContainsKey(propertyName + "_g") && Properties.ContainsKey(propertyName + "_b"))
                return Color.FromArgb((Byte)Properties[propertyName + "_a"], (Byte)Properties[propertyName + "_r"], (Byte)Properties[propertyName + "_g"], (Byte)Properties[propertyName + "_b"]);
            return defaultValue;
        }

        public void Set(String propertyName, Color? value)
        {
            if (!value.HasValue)
            {
                Properties.Remove(propertyName + "_a");
                Properties.Remove(propertyName + "_r");
                Properties.Remove(propertyName + "_g");
                Properties.Remove(propertyName + "_b");
                return;
            }

            if (!Properties.ContainsKey(propertyName + "_a"))
                Properties.Add(propertyName + "_a", value.Value.A);
            else
                Properties[propertyName + "_a"] = value.Value.A;
            if (!Properties.ContainsKey(propertyName + "_r"))
                Properties.Add(propertyName + "_r", value.Value.R);
            else
                Properties[propertyName + "_r"] = value.Value.R;
            if (!Properties.ContainsKey(propertyName + "_g"))
                Properties.Add(propertyName + "_g", value.Value.G);
            else
                Properties[propertyName + "_g"] = value.Value.G;
            if (!Properties.ContainsKey(propertyName + "_b"))
                Properties.Add(propertyName + "_b", value.Value.B);
            else
                Properties[propertyName + "_b"] = value.Value.B;
        }

        public FontStyle Get(String propertyName, FontStyle defaultValue)
        {
            if (Properties.ContainsKey(propertyName))
                return (FontStyle)((Byte)Properties[propertyName]);
            return defaultValue;
        }

        public void Set(String propertyName, FontStyle value)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, (Byte)value);
            else
                Properties[propertyName] = (Byte)value;
        }

        public FontWeight Get(String propertyName, FontWeight defaultValue)
        {
            if (!Properties.ContainsKey(propertyName))
                return defaultValue;

            switch ((Byte)Properties[propertyName])
            {
                case 0:
                    return FontWeights.Black;
                case 1:
                    return FontWeights.Bold;
                case 2:
                    return FontWeights.ExtraBlack;
                case 3:
                    return FontWeights.ExtraBold;
                case 4:
                    return FontWeights.ExtraLight;
                case 5:
                    return FontWeights.Light;
                case 6:
                    return FontWeights.Medium;
                case 7:
                    return FontWeights.Normal;
                case 8:
                    return FontWeights.SemiBold;
                case 9:
                    return FontWeights.SemiLight;
                case 10:
                    return FontWeights.Thin;
                default:
                    return defaultValue;
            }
        }

        public void Set(String propertyName, FontWeight value)
        {
            var weightValue = (Byte)4;
            if (value.Equals(FontWeights.Black))
                weightValue = 0;
            else if (value.Equals(FontWeights.Bold))
                weightValue = 1;
            else if (value.Equals(FontWeights.ExtraBlack))
                weightValue = 2;
            else if (value.Equals(FontWeights.ExtraBold))
                weightValue = 3;
            else if (value.Equals(FontWeights.ExtraLight))
                weightValue = 4;
            else if (value.Equals(FontWeights.Light))
                weightValue = 5;
            else if (value.Equals(FontWeights.Medium))
                weightValue = 6;
            else if (value.Equals(FontWeights.Normal))
                weightValue = 7;
            else if (value.Equals(FontWeights.SemiBold))
                weightValue = 8;
            else if (value.Equals(FontWeights.SemiLight))
                weightValue = 9;
            else if (value.Equals(FontWeights.Thin))
                weightValue = 10;

            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, weightValue);
            else
                Properties[propertyName] = weightValue;
        }
    }
}