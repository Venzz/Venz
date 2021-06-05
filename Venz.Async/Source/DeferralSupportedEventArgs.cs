using System;
using System.Threading;
using System.Threading.Tasks;

namespace Venz.Async
{
    public class DeferralSupportedEventArgs: EventArgs
    {
        private InvocationCounter Counter;

        public IDeferral GetDeferral()
        {
            if (Counter == null)
                Counter = new InvocationCounter();

            var deferral = new Deferral(Counter);
            Counter.Increment();
            return deferral;
        }

        public Task WaitCompletionAsync() => (Counter == null) ? Task.FromResult(true) : Counter.Awaiter.Task;



        public interface IDeferral { void Complete(); }

        private class Deferral: IDeferral
        {
            private InvocationCounter Counter;
            public Deferral(InvocationCounter counter) { Counter = counter; }
            public void Complete() => Counter.Decrement();
        }

        private class InvocationCounter
        {
            private Int32 Value;
            public TaskCompletionSource<Boolean> Awaiter = new TaskCompletionSource<Boolean>();
            public void Increment() => Interlocked.Increment(ref Value);
            public void Decrement() { Interlocked.Decrement(ref Value); if (Value == 0) Awaiter.TrySetResult(true); }
        }
    }
}