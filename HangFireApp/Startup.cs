using Hangfire;
using Hangfire.Dashboard;
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

            //Use the same way to limit the number of attempts to the different value.
            //If you want to change the default global value, add a new global filter:
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });

            //Hangfire Server periodically checks the schedule to enqueue scheduled jobs to their queues,
            //allowing workers to execute them. By default, check interval is equal to 15 seconds,
            //but you can change it by setting the SchedulePollingInterval property on the options you pass to the BackgroundJobServer constructor:
            //var options = new BackgroundJobServerOptions
            //{
            //    SchedulePollingInterval = TimeSpan.FromMinutes(1)
            //};
            //yield return new BackgroundJobServer(options);

            var options = new BackgroundJobServerOptions
            {
                Queues = new[] { "a_zatca", "b_nphies", "c_sms", "d_email","e_daily","f_clean_up", "g_noshow" }
            };

            yield return new BackgroundJobServer(options);
        }

        public void Configuration(IAppBuilder app)
        {
            //app.UseHangfireAspNet(GetHangfireServers);
            //app.UseHangfireDashboard();

            //var options = new DashboardOptions
            //{
            //    Authorization = new[]{new LocalRequestsOnlyAuthorizationFilter() }
            //};

            //app.UseHangfireDashboard("/hangfire", options);
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            //// Let's also create a sample background job
            BackgroundJob.Schedule(() => TestMethod(), TimeSpan.FromMinutes(1));

            var jobScheduler = new HangfireJobScheduler();
            jobScheduler.ScheduleRecurringJob();

        }

        [Queue("a_default")]
        public void TestMethod()
        {
            Debug.WriteLine("Hello world from Hangfire!");
        }
    }
}
