using System;
using Venz.UI.Xaml;

namespace Venz.UI
{
    public class BooleanTo<T>: OneWayValueConverter<Boolean>
    {
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }
        protected override Object Convert(Boolean value) => value ? TrueValue : FalseValue;
    }
}
