using System;
using Windows.Foundation;

namespace Venz.Data
{
    public enum IncrementalLoadingState { Normal, Loading, WaitingForUserInput, Finished }

    public enum IncrementalLoadingTriggerLocation { Bottom, Top }

    public class LoadedItemsReport
    {
        public Boolean Final { get; private set; }
        public Boolean Failed { get; private set; }

        private LoadedItemsReport() { }
        public static LoadedItemsReport CreateSuccessful(Boolean final) => new LoadedItemsReport() { Final = final };
        public static LoadedItemsReport CreateFailed() => new LoadedItemsReport() { Failed = true };
    }

    public interface ISupportIncrementalLoading
    {
        Boolean HasMoreItems { get; }
        IncrementalLoadingTriggerLocation TriggerLocation { get; }

        event TypedEventHandler<ISupportIncrementalLoading, UInt32> MoreItemsRequested;
        event TypedEventHandler<ISupportIncrementalLoading, IncrementalLoadingState> StateChanged;

        void RequestMoreItems(UInt32 count);
        void RetryRequestMoreItems();
    }
}
