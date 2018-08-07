using System;
using Venz.UI.Xaml;

namespace Venz.UI
{
    public class BooleanTo<T>: OneWayValueConverter<Boolean, T>
    {
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }
        public override T Convert(Boolean value) => value ? TrueValue : FalseValue;
    }
}
