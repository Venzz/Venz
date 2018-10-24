using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Venz.Async
{
    public class Conveyor<T> where T: class
    {
        private Object Sync;
        private List<T> PendingItems;
        private ItemAction Action;
        private CancellationTokenSource ProcessingItemCancellationToken;

        public Int32 Count => PendingItems.Count;
        public T ProcessingItem { get; private set; }
        public event TypedEventHandler<Conveyor<T>, T> ItemRemoved = delegate { };
        public event TypedEventHandler<Conveyor<T>, ExceptionEventArgs<T>> Exception = delegate { };



        public Conveyor(ItemAction action)
        {
            Sync = new Object();
            Action = action;
            PendingItems = new List<T>();
        }

        public Conveyor(ItemAction action, IEnumerable<T> items)
        {
            Sync = new Object();
            Action = action;
            PendingItems = new List<T>(items);
        }

        public IEnumerable<T> GetItems()
        {
            lock (Sync)
            {
                var items = new List<T>(PendingItems);
                if (ProcessingItem != null)
                    items.Insert(0, ProcessingItem);
                return items;
            }
        }

        public void Start()
        {
            Task.Run(() => Processor());
        }

        public void Enqueue(T item)
        {
            lock (Sync)
            {
                PendingItems.Add(item);
                Monitor.Pulse(Sync);
            }
        }

        public void Enqueue(IEnumerable<T> items)
        {
            lock (Sync)
            {
                PendingItems.AddRange(items);
                if (PendingItems.Count > 0)
                    Monitor.Pulse(Sync);
            }
        }

        public DequeueOperationStatus Dequeue(T item)
        {
            lock (Sync)
            {
                if (ProcessingItem == item)
                {
                    ProcessingItemCancellationToken.Cancel();
                    return DequeueOperationStatus.Cancelling;
                }
                else if (PendingItems.Remove(item))
                {
                    ItemRemoved(this, item);
                    return DequeueOperationStatus.Removed;
                }
                else
                {
                    return DequeueOperationStatus.NotExist;
                }
            }
        }

        public void Clear()
        {
            lock (Sync)
            {
                ProcessingItemCancellationToken?.Cancel();
                PendingItems.Clear();
            }
        }

        private async void Processor()
        {
            while (true)
            {
                lock (Sync)
                {
                    if (PendingItems.Count == 0)
                        Monitor.Wait(Sync);

                    ProcessingItem = PendingItems[0];
                    PendingItems.RemoveAt(0);
                    ProcessingItemCancellationToken = new CancellationTokenSource();
                }

                try
                {
                    await Action.Invoke(ProcessingItem, ProcessingItemCancellationToken.Token);
                }
                catch (Exception exception)
                {
                    Exception(this, new ExceptionEventArgs<T>(ProcessingItem, exception));
                }
                finally
                {
                    ItemRemoved(this, ProcessingItem);
                    ProcessingItem = default(T);
                    ProcessingItemCancellationToken = null;
                }
            }
        }

        public delegate Task ItemAction(T item, CancellationToken cancellation);

        public enum DequeueOperationStatus { Cancelling, Removed, NotExist }

        public class ExceptionEventArgs<TItem>: EventArgs where TItem: class
        {
            public TItem Item { get; }
            public Exception Exception { get; }

            public ExceptionEventArgs(TItem item, Exception exception)
            {
                Item = item;
                Exception = exception;
            }
        }
    }
}
