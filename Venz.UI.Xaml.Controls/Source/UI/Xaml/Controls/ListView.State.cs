using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Venz.UI.Xaml.Controls
{
    public partial class ListView
    {
        public String GetState(Func<Object, String> itemToKeyFunction)
        {
            var itemToKeyHandler = (ListViewItemToKeyHandler)((item) => itemToKeyFunction.Invoke(item));
            return ListViewPersistenceHelper.GetRelativeScrollPosition(this, itemToKeyHandler);
        }

        public Task SetStateAsync(String state, Func<String, Object> keyToItemFunction)
        {
            ListViewKeyToItemHandler keyToItemHandler = (key) => LoadedStateAwaiter.Task.ContinueWith(t => String.IsNullOrEmpty(key) ? null : keyToItemFunction.Invoke(key)).AsAsyncOperation();
            return ListViewPersistenceHelper.SetRelativeScrollPositionAsync(this, state, keyToItemHandler).AsTask();
        }
    }
}
