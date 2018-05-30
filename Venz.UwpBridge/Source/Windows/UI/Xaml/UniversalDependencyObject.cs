using System;

namespace Windows.UI.Xaml
{
    public class UniversalDependencyObject
    {
        public static Int64 RegisterPropertyChangedCallback(DependencyObject dependencyObject, DependencyProperty dp, String dpName, DependencyPropertyChangedCallback callback)
        {
            DependencyPropertyChangesListener.Create(dependencyObject, dp, dpName, callback);
            return 0;
        }

        public delegate void DependencyPropertyChangedCallback(DependencyObject sender, DependencyProperty dp);

        private class DependencyPropertyChangesListener: DependencyObject, IDisposable
        {
            private DependencyObject Target;
            private DependencyProperty Property;
            private DependencyPropertyChangedCallback Callback;

            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(Object), typeof(DependencyObject),
                new PropertyMetadata(null, (obj, args) => ((DependencyPropertyChangesListener)obj).OnValueChanged()));



            private DependencyPropertyChangesListener() { }

            public static void Create(DependencyObject target, DependencyProperty property, String propertyName, DependencyPropertyChangedCallback callback)
            {
                var listener = new DependencyPropertyChangesListener();
                listener.Target = target;
                listener.Property = property;
                listener.Callback = callback;

                var binding = new Data.Binding();
                binding.Source = target;
                binding.Mode = Data.BindingMode.OneWay;
                binding.Path = new PropertyPath(propertyName);
                Data.BindingOperations.SetBinding(listener, ValueProperty, binding);
            }

            private void OnValueChanged() => Callback.Invoke(Target, Property);

            public void Dispose() => ClearValue(ValueProperty);
        }
    }
}
