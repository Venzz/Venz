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

        public event TypedEventHandler<ItemsSourceModificationListener, Args> Changed;



        public ItemsSourceModificationListener() { }

        public void ChangeCollection(Object collection)
        {
            if ((collection != null) && !(collection is IEnumerable<Object>))
                throw new ArgumentException();

            if (ObservableCollection != null)
                ObservableCollection.CollectionChanged -= OnCollectionChanged;
            if (ObservableVector != null)
                ObservableVector.VectorChanged -= OnVectorChanged;

            var oldCollection = Collection;
            Collection = (IEnumerable<Object>)collection;
            ObservableCollection = (collection is INotifyCollectionChanged) ? (INotifyCollectionChanged)collection : null;
            ObservableVector = (ObservableCollection == null) && (collection is IObservableVector<Object>) ? (IObservableVector<Object>)collection : null;

            if (ObservableCollection != null)
                ObservableCollection.CollectionChanged += OnCollectionChanged;
            if (ObservableVector != null)
                ObservableVector.VectorChanged += OnVectorChanged;

            var oldCount = Count;
            Count = (Collection == null) ? 0U : (UInt32)Collection.Count();
            if ((oldCollection != Collection) || (oldCount != Count))
                Changed?.Invoke(this, new Args() { OldCollection = oldCollection, NewCollection = Collection, OldCount = oldCount, NewCount = Count });
        }

        private void OnVectorChanged(IObservableVector<Object> sender, IVectorChangedEventArgs args)
        {
            var count = (UInt32)ObservableVector.Count;
            var oldCount = Count;
            if (Count != count)
            {
                Count = count;
                Changed?.Invoke(this, new Args() { OldCollection = Collection, NewCollection = Collection, OldCount = oldCount, NewCount = Count });
            }
        }

        private void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args)
        {
            var count = (UInt32)Collection.Count();
            var oldCount = Count;
            if (Count != count)
            {
                Count = count;
                Changed?.Invoke(this, new Args() { OldCollection = Collection, NewCollection = Collection, OldCount = oldCount, NewCount = Count });
            }
        }

        public class Args
        {
            internal Args() { }
            public IEnumerable<Object> OldCollection { get; internal set; }
            public IEnumerable<Object> NewCollection { get; internal set; }
            public UInt32 OldCount { get; internal set; }
            public UInt32 NewCount { get; internal set; }
            public Boolean CountChanged => OldCount != NewCount;
            public Boolean CollectionChanged => OldCollection != NewCollection;
        }
    }
}