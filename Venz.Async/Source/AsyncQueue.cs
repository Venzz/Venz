using System;
using System.Threading;
using System.Threading.Tasks;

namespace Venz.Async
{
    public class AsyncQueue: IDisposable
    {
        private Boolean IsDisposed;
        private Task LastTask = Task.CompletedTask;
        private CancellationTokenSource Cancellation = new CancellationTokenSource();

        public void Add(Action<CancellationToken> action)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(AsyncQueue));

            LastTask = LastTask.ContinueWith((task) => action.Invoke(Cancellation.Token), Cancellation.Token, TaskContinuationOptions.None, TaskScheduler.Default);
        }

        public Task RunAsync(Action<CancellationToken> action)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(AsyncQueue));

            LastTask = LastTask.ContinueWith((task) => action.Invoke(Cancellation.Token), Cancellation.Token, TaskContinuationOptions.None, TaskScheduler.Default);
            return LastTask;
        }

        public Task RunAsync(Func<CancellationToken, Task> action)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(AsyncQueue));

            LastTask = LastTask.ContinueWith((task) => action.Invoke(Cancellation.Token), Cancellation.Token, TaskContinuationOptions.None, TaskScheduler.Default).Unwrap();
            return LastTask;
        }

        public Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> action)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(AsyncQueue));

            var actionTask = LastTask.ContinueWith((task) => action.Invoke(Cancellation.Token), Cancellation.Token, TaskContinuationOptions.None, TaskScheduler.Default).Unwrap();
            LastTask = actionTask;
            return actionTask;
        }

        public Task<T> RunAsync<T>(Func<CancellationToken, T> action)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(AsyncQueue));

            var actionTask = LastTask.ContinueWith((task) => action.Invoke(Cancellation.Token), Cancellation.Token, TaskContinuationOptions.None, TaskScheduler.Default);
            LastTask = actionTask;
            return actionTask;
        }

        /*public void Run(Action<CancellationToken> action, TaskScheduler scheduler)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(AsyncQueue));

            LastTask = LastTask.ContinueWith((task) => action.Invoke(Cancellation.Token), Cancellation.Token, TaskContinuationOptions.None, scheduler);
        }*/

        public void Dispose()
        {
            IsDisposed = true;
            Cancellation.Cancel();
        }
    }
}
