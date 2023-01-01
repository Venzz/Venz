using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Venz.Async
{
    public class Conveyor<T> where T: class
    {
        private List<T> PendingItems;
        private ItemAction Action;
        private Cancellation ProcessingItemCancellation;

        public Object Sync { get; } = new Object();
        public Int32 Count => PendingItems.Count;
        public T ProcessingItem { get; private set; }
        public event TypedEventHandler<Conveyor<T>, T> ItemRemoved = delegate { };
        public event TypedEventHandler<Conveyor<T>, ExceptionEventArgs<T>> Exception = delegate { };



        public Conveyor(ItemAction action)
        {
            Action = action;
            PendingItems = new List<T>();
        }

        public Conveyor(ItemAction action, IEnumerable<T> items)
        {
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

        public void Enqueue(T item, ConveyorPriority priority = ConveyorPriority.Default) => Enqueue(new List<T>() { item }, priority);

        public void Enqueue(IEnumerable<T> items, ConveyorPriority priority = ConveyorPriority.Default)
        {
            lock (Sync)
            {
                if (priority == ConveyorPriority.High)
                {
                    foreach (var item in items)
                        PendingItems.Remove(item);
                    foreach (var item in items.Reverse())
                        PendingItems.Insert(0, item);
                }
                else
                {
                    PendingItems.AddRange(items);
                }
                if (PendingItems.Count > 0)
                {
                    Monitor.Pulse(Sync);
                }
            }
        }

        public DequeueOperationStatus Dequeue(T item)
        {
            lock (Sync)
            {
                if (ProcessingItem == item)
                {
                    ProcessingItemCancellation.Cancel();
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
                ProcessingItemCancellation?.Cancel();
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
                    if (PendingItems.Count == 0)
                        continue;

                    ProcessingItem = PendingItems[0];
                    PendingItems.RemoveAt(0);
                    ProcessingItemCancellation = new Cancellation();
                }

                try
                {
                    await Action.Invoke(ProcessingItem, ProcessingItemCancellation);
                }
                catch (Exception exception)
                {
                    Exception(this, new ExceptionEventArgs<T>(ProcessingItem, exception));
                }
                finally
                {
                    ItemRemoved(this, ProcessingItem);
                    ProcessingItem = default(T);
                    ProcessingItemCancellation = null;
                }
            }
        }



        public delegate Task ItemAction(T item, Cancellation cancellation);

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

        public class Cancellation
        {
            public Boolean IsCancellationRequested { get; private set; }
            public void Cancel() => IsCancellationRequested = true;
        }
    }
}
