using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Venz.Data
{
    public class ObservableList<T>: IList<T>, IList, INotifyCollectionChanged
    {
        protected List<T> Items;

        public Object SyncRoot => new Object();
        public Boolean IsReadOnly => throw new NotImplementedException();
        public Boolean IsFixedSize => throw new NotImplementedException();
        public Boolean IsSynchronized => true;
        public Int32 Count => Items.Count;
        public T this[Int32 index] { get => Items[index]; set => throw new NotImplementedException(); }
        Object IList.this[Int32 index] { get => Items[index]; set => throw new NotImplementedException(); }

        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };



        public ObservableList()
        {
            Items = new List<T>();
        }

        public ObservableList(IEnumerable<T> initialItems)
        {
            Items = new List<T>(initialItems);
        }



        public void Add(T item)
        {
            Items.Add(item);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Count - 1));
        }

        public Int32 Add(Object value)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Insert(Int32 index, T item)
        {
            lock (SyncRoot)
            {
                Items.Insert(index, item);
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        public void Insert(Int32 index, Object value) => throw new NotImplementedException();

        public void Move(Int32 oldIndex, Int32 newIndex)
        {
            var removedItem = Items[oldIndex];
            Items.RemoveAt(oldIndex);
            if (newIndex > oldIndex)
                newIndex--;
            Items.Insert(newIndex, removedItem);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, oldIndex));
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, removedItem, newIndex));
        }

        public Boolean Remove(T item) => throw new NotImplementedException();

        public void Remove(Object value) => throw new NotImplementedException();

        public void RemoveAt(Int32 index)
        {
            var removedItem = Items[index];
            Items.RemoveAt(index);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        public void Clear() => throw new NotImplementedException();

        public Int32 IndexOf(T item) => Items.IndexOf(item);

        public UInt32? IndexOf(Predicate<T> match)
        {
            var index = Items.FindIndex(match);
            return (index == -1) ? null : (UInt32?)index;
        }

        public Int32 IndexOf(Object value) => (value is T) ? Items.IndexOf((T)value) : -1;

        public Boolean Contains(T item) => throw new NotImplementedException();

        public Boolean Contains(Object value) => throw new NotImplementedException();

        public void CopyTo(T[] array, Int32 arrayIndex) => throw new NotImplementedException();

        public void CopyTo(Array array, Int32 index) => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }
}
