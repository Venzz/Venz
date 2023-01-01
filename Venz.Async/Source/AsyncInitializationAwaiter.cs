using System;
using System.Threading.Tasks;

namespace Venz.Async
{
    public class AsyncInitializationAwaiter: IDisposable
    {
        private IAsyncInitializable AsyncInitializable;
        private TaskCompletionSource<Object> InitializationAwaiter = new TaskCompletionSource<Object>(TaskCreationOptions.RunContinuationsAsynchronously);

        public AsyncInitializationAwaiter(IAsyncInitializable asyncInitializable)
        {
            AsyncInitializable = asyncInitializable;
        }

        public Task EnsureInitializedAsync()
        {
            lock (AsyncInitializable.Sync)
            {
                if (!AsyncInitializable.Initialized)
                    AsyncInitializable.InitializationCompleted += OnInitializationCompleted;
                else
                    InitializationAwaiter.SetResult(true);
            }
            return InitializationAwaiter.Task;
        }

        private void OnInitializationCompleted(IAsyncInitializable sender, Object args)
        {
            AsyncInitializable.InitializationCompleted -= OnInitializationCompleted;
            InitializationAwaiter.SetResult(null);
        }

        public void Dispose()
        {
            lock (AsyncInitializable.Sync)
            {
                AsyncInitializable.InitializationCompleted -= OnInitializationCompleted;
            }
        }
    }
}
