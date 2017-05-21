using System;
using System.Threading.Tasks;

namespace Venz.Async
{
    public class AsyncEvent
    {
        private TaskCompletionSource<Boolean> Awaiter = new TaskCompletionSource<Boolean>();

        public Boolean Occurred => Awaiter.Task.IsCompleted;



        public void Signal() => Awaiter.SetResult(true);

        public void SignalAndContinue() => Awaiter.SetResultAndContinue(true);

        public Task WaitAsync() => Awaiter.Task;
    }
}
