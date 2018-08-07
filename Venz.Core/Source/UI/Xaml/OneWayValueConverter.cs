using System;
using Windows.UI.Xaml.Data;

namespace Venz.UI.Xaml
{
    public abstract class OneWayValueConverter<TInput, TOutput>: IValueConverter
    {
        public abstract TOutput Convert(TInput convertingValue);

        public Object Convert(Object value, Type targetType, Object parameter, String language) => Convert((TInput)value);

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) => throw new NotImplementedException();
    }
}
