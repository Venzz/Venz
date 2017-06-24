using System;
using System.Threading.Tasks;

namespace Venz.Background
{
    public interface IBackgroundApplication
    {
        Task StartAsync(IBackgroundTaskCancellation taskCancellation, Object taskTriggerDetails);
        void OnCanceled();
    }
}
