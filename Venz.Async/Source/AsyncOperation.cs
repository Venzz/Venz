using System;
using System.Threading.Tasks;

namespace Venz.Async
{
    public class AsyncOperation<T>
    {
        private Object Sync = new Object();
        private AsyncOperationMode Mode;
        private Func<T, Task> ParameterizedOperationGetter;
        private Func<Task<T>> OperationGetter;
        private Task ActiveOperation;

        public AsyncOperation(Func<T, Task> operationGetter, AsyncOperationMode mode)
        {
            Mode = mode;
            ParameterizedOperationGetter = operationGetter;
        }

        public AsyncOperation(Func<Task<T>> operationGetter)
        {
            OperationGetter = operationGetter;
        }

        public Task<T> PerformAsync()
        {
            if (OperationGetter == null)
                throw new InvalidOperationException();

            lock (Sync)
            {
                if (ActiveOperation == null)
                {
                    ActiveOperation = OperationGetter.Invoke();
                    return (Task<T>)ActiveOperation;
                }
                else if (ActiveOperation.IsCompleted || ActiveOperation.IsCanceled || ActiveOperation.IsFaulted)
                {
                    ActiveOperation = OperationGetter.Invoke();
                    return (Task<T>)ActiveOperation;
                }
                else
                {
                    return (Task<T>)ActiveOperation;
                }
            }
        }

        public Task PerformAsync(T parameter)
        {
            if (ParameterizedOperationGetter == null)
                throw new InvalidOperationException();

            lock (Sync)
            {
                if (ActiveOperation == null)
                {
                    ActiveOperation = ParameterizedOperationGetter.Invoke(parameter);
                    return ActiveOperation;
                }
                else if ((Mode == AsyncOperationMode.OneTime) && ActiveOperation.IsCompleted)
                {
                    return Task.CompletedTask;
                }
                else if (ActiveOperation.IsCompleted || ActiveOperation.IsCanceled || ActiveOperation.IsFaulted)
                {
                    ActiveOperation = ParameterizedOperationGetter.Invoke(parameter);
                    return ActiveOperation;
                }
                else
                {
                    return ActiveOperation;
                }
            }
        }
    }
}
