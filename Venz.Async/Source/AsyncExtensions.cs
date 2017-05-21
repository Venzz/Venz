using System;
using System.Threading.Tasks;

namespace Venz.Async
{
    public static class AsyncExtensions
    {
        public static void SetResultAndContinue<T>(this TaskCompletionSource<T> value, T result)
        {
            Task.Run(() => value.SetResult(result));
        }

        public static void SetExceptionAndContinue<T>(this TaskCompletionSource<T> value, Exception exception)
        {
            Task.Run(() => value.SetException(exception));
        }
    }
}
