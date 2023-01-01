using System;
using Windows.Foundation;

namespace Venz.Async
{
    public interface IAsyncInitializable
    {
        Object Sync { get; }
        Boolean Initialized { get; }

        event TypedEventHandler<IAsyncInitializable, Object> InitializationCompleted;
    }
}
