using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Venz.Windows
{
    public static class BackgroundTask
    {
        public static async Task<Boolean> RequestAccessAsync()
        {
            var result = await BackgroundExecutionManager.RequestAccessAsync();
            return ((result == BackgroundAccessStatus.AllowedSubjectToSystemPolicy) || (result == BackgroundAccessStatus.AlwaysAllowed));
        }

        public static void Register(IBackgroundTrigger taskTrigger, String name, String entryPoint)
        {
            Register(taskTrigger, name, entryPoint, null);
        }

        public static void Register(IBackgroundTrigger taskTrigger, String name, String entryPoint, IBackgroundCondition condition)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
                if (task.Value.Name == name)
                    return;

            var builder = new BackgroundTaskBuilder();
            builder.Name = name;
            builder.TaskEntryPoint = entryPoint;
            builder.SetTrigger(taskTrigger);
            if (condition != null)
                builder.AddCondition(condition);
            builder.Register();
        }
    }
}
