using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Venz.Telemetry
{
    internal static class TaskExtensions
    {
        public static IAsyncAction CompletedAction => Task.FromResult<Object>(null).AsAsyncAction();
    }
}
