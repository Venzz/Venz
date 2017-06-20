using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Venz.Data
{
    public class ObservableObject: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };



        protected async void OnPropertyChanged(params String[] propertyNames) => await OnPropertyChangedAsync(propertyNames);

        protected async void OnPropertyChanged(IEnumerable<String> propertyNames) => await OnPropertyChangedAsync(propertyNames);

        protected Task OnPropertyChangedAsync(params String[] propertyNames) => OnPropertyChangedAsync((IEnumerable<String>)propertyNames);

        protected async Task OnPropertyChangedAsync(IEnumerable<String> propertyNames)
        {
            try
            {
                var dispatcher = Window.Current?.Dispatcher;
                if (dispatcher != null)
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => OnPropertyChangedInternal(propertyNames)).AsTask().ConfigureAwait(false);
                else
                    OnPropertyChanged(propertyNames);
            }
            #if DEBUG
            catch (Exception exception)
            {
                throw exception;
            }
            #else
            catch (Exception)
            {
            }
            #endif
        }



        private void OnPropertyChangedInternal(params String[] propertyNames) => OnPropertyChangedInternal((IEnumerable<String>)propertyNames);

        private void OnPropertyChangedInternal(IEnumerable<String> propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
