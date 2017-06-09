using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Venz.UI.Xaml
{
    public class ItemsSourceModificationListener
    {
        private IEnumerable<Object> Collection;
        private INotifyCollectionChanged ObservableCollection;
        private IObservableVector<Object> ObservableVector;

        public UInt32 Count { get; private set; }

        public event TypedEventHandler<ItemsSourceModificationListener, UInt32> Changed;



        public ItemsSourceModificationListener() { }

        public void ChangeCollection(Object collection)
        {
            if ((collection != null) && !(collection is IEnumerable<Object>))
                throw new ArgumentException();

            if (ObservableCollection != null)
                ObservableCollection.CollectionChanged -= OnCollectionChanged;
            if (ObservableVector != null)
                ObservableVector.VectorChanged -= OnVectorChanged;

            Collection = (IEnumerable<Object>)collection;
            ObservableCollection = (collection is INotifyCollectionChanged) ? (INotifyCollectionChanged)collection : null;
            ObservableVector = (ObservableCollection == null) && (collection is IObservableVector<Object>) ? (IObservableVector<Object>)collection : null;

            if (ObservableCollection != null)
                ObservableCollection.CollectionChanged += OnCollectionChanged;
            if (ObservableVector != null)
                ObservableVector.VectorChanged += OnVectorChanged;

            var count = (Collection == null) ? 0U : (UInt32)Collection.Count();
            if (Count != count)
            {
                Count = count;
                Changed?.Invoke(this, Count);
            }
        }

        private void OnVectorChanged(IObservableVector<Object> sender, IVectorChangedEventArgs args)
        {
            var count = (UInt32)ObservableVector.Count;
            if (Count != count)
            {
                Count = count;
                Changed?.Invoke(this, Count);
            }
        }

        private void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args)
        {
            var count = (UInt32)Collection.Count();
            if (Count != count)
            {
                Count = count;
                Changed?.Invoke(this, Count);
            }
        }
    }
}