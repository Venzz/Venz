using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Venz.Images
{
    internal class RequestProcessor
    {
        private Object Sync = new Object();
        private TaskFactory ImageRenderingTaskFactory;

        private List<IRequest> PendingRequests = new List<IRequest>();
        private List<IRequest> PendingHighPriorityRequests = new List<IRequest>();

        public RequestProcessor(TaskScheduler imageRenderingTaskScheduler)
        {
            ImageRenderingTaskFactory = new TaskFactory(imageRenderingTaskScheduler);
            Task.Run(() => ProcessAsync());
        }

        public void Enqueue(IRequest request, Int32 priority)
        {
            lock (Sync)
            {
                if (priority == 0)
                    PendingRequests.Add(request);
                else
                    PendingHighPriorityRequests.Add(request);
                Monitor.Pulse(Sync);
            }
        }

        public void Dequeue(Picture picture)
        {
            lock (Sync)
            {
                for (var i = 0; i < PendingRequests.Count; i++)
                {
                    if (PendingRequests[i].IsRequestFor(picture))
                    {
                        PendingRequests.RemoveAt(i);
                        break;
                    }
                }
                for (var i = 0; i < PendingHighPriorityRequests.Count; i++)
                {
                    if (PendingHighPriorityRequests[i].IsRequestFor(picture))
                    {
                        PendingHighPriorityRequests.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void Dequeue(Object tag)
        {
            lock (Sync)
            {
                PendingRequests.RemoveAll(a => Object.Equals(a.Tag, tag));
                PendingHighPriorityRequests.RemoveAll(a => Object.Equals(a.Tag, tag));
            }
        }

        private async Task ProcessAsync()
        {
            while (true)
            {
                lock (Sync)
                    if ((PendingRequests.Count == 0) && (PendingHighPriorityRequests.Count == 0))
                        Monitor.Wait(Sync);

                var request = (PendingHighPriorityRequests.Count > 0) ? PendingHighPriorityRequests[0] : null;
                if (request == null)
                    request = (PendingRequests.Count > 0) ? PendingRequests[0] : null;
                if (request == null)
                    continue;

                try
                {
                    var removeFromQueue = await request.ProcessAsync(ImageRenderingTaskFactory).ConfigureAwait(false);
                    if (removeFromQueue)
                    {
                        PendingHighPriorityRequests.Remove(request);
                        PendingRequests.Remove(request);
                    }
                }
                catch (Exception)
                {
                    PendingHighPriorityRequests.Remove(request);
                    PendingRequests.Remove(request);
                }
            }
        }
    }
}
