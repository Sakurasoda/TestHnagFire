using System;
using System.ComponentModel;
using System.Threading;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Server;

namespace WebApplication
{
    [ManagementPage("演示")]
    public class Job
    {
        [Hangfire.Dashboard.Management.Support.Job]
        [DisplayName("呼叫內部方法")]
        public void Action(PerformContext context = null, IJobCancellationToken cancellationToken = null)
        {
            if (cancellationToken.ShutdownToken.IsCancellationRequested)
            {
                return;
            }
 
            context.WriteLine($"測試用，Now:{DateTime.Now}");
            Thread.Sleep(30000);
        }
    }
}