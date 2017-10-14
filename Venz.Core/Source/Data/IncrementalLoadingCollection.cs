using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Venz.Data
{
    public class IncrementalLoadingCollection<T>: ObservableList<T>, ISupportIncrementalLoading
    {
        private UInt32? RequestedItemsCount;

        public Boolean HasMoreItems { get; private set; } = true;
        public IncrementalLoadingTriggerLocation TriggerLocation { get; set; }

        public event TypedEventHandler<ISupportIncrementalLoading, UInt32> MoreItemsRequested = delegate { };
        public event TypedEventHandler<ISupportIncrementalLoading, IncrementalLoadingState> StateChanged = delegate { };



        public IncrementalLoadingCollection() { }

        public IncrementalLoadingCollection(IEnumerable<T> initialItems): base(initialItems) { }

        public void RequestMoreItems(UInt32 count)
        {
            lock (SyncRoot)
            {
                if (RequestedItemsCount.HasValue)
                    return;

                RequestedItemsCount = count;
                MoreItemsRequested(this, count);
                StateChanged(this, IncrementalLoadingState.Loading);
            }
        }

        public void RetryRequestMoreItems()
        {
            lock (SyncRoot)
            {
                if (RequestedItemsCount.HasValue)
                {
                    MoreItemsRequested(this, RequestedItemsCount.Value);
                    StateChanged(this, IncrementalLoadingState.Loading);
                }
            }
        }

        public void ReportLoadedItems(LoadedItemsReport loadedItemsReport)
        {
            lock (SyncRoot)
            {
                if (loadedItemsReport.Failed)
                {
                    StateChanged(this, IncrementalLoadingState.WaitingForUserInput);
                    return;
                }

                if (HasMoreItems)
                    HasMoreItems = !loadedItemsReport.Final;

                RequestedItemsCount = null;
                StateChanged(this, HasMoreItems ? IncrementalLoadingState.Normal : IncrementalLoadingState.Finished);
            }
        }
    }
}
