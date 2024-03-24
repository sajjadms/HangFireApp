using Hangfire;
using HangFireApp.Extensions;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(HangFireApp.Startup))]

namespace HangFireApp
{
    public class Startup
    {
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("Data Source=(local);Initial Catalog=HangfireTest;Integrated Security=True;TrustServerCertificate=True;")
                .UseFilter(new LogFailureAttribute());


            //Hangfire Server periodically checks the schedule to enqueue scheduled jobs to their queues,
            //allowing workers to execute them. By default, check interval is equal to 15 seconds,
            //but you can change it by setting the SchedulePollingInterval property on the options you pass to the BackgroundJobServer constructor:
            //var options = new BackgroundJobServerOptions
            //{
            //    SchedulePollingInterval = TimeSpan.FromMinutes(1)
            //};
            //yield return new BackgroundJobServer(options);

            yield return new BackgroundJobServer();
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard();

            //// Let's also create a sample background job
            //BackgroundJob.Enqueue(() => Debug.WriteLine("Hello world from Hangfire!"));

            var jobScheduler = new HangfireJobScheduler();
            jobScheduler.ScheduleRecurringJob();

        }
    }
}
