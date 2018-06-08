using System;
using Windows.UI.Xaml.Data;

namespace Venz.UI.Xaml
{
    public abstract class OneWayValueConverter<T>: IValueConverter
    {
        protected abstract Object Convert(T convertingValue);

        public Object Convert(Object value, Type targetType, Object parameter, String language) { return Convert((T)value); }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) { throw new NotImplementedException(); }
    }
}
