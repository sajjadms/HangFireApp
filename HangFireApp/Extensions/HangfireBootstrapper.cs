using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace HangFireApp.Extensions
{
    public class HangfireBootstrapper : IRegisteredObject
    {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;

        private BackgroundJobServer _backgroundJobServer;

        private HangfireBootstrapper()
        {
        }

        public void Start()
        {
            lock (_lockObject)
            {
                if (_started) return;
                _started = true;

                HostingEnvironment.RegisterObject(this);

                GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["HangFireConn"].ConnectionString, 
                new SqlServerStorageOptions
                {
                    JobExpirationCheckInterval = TimeSpan.FromDays(1), // Check expiration daily
                                                                       //This configuration setting in SqlServerStorageOptions determines how often Hangfire checks for and
                                                                       //removes expired jobs from the storage. It is a background process managed by Hangfire itself,
                                                                       //and it runs independently of any scheduled jobs.
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true // Migration to Schema 7 is required


                })
                .UseFilter(new LogFailureAttribute());

                //Use the same way to limit the number of attempts to the different value.
                //If you want to change the default global value, add a new global filter:
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });

                GlobalJobFilters.Filters.Add(new ProlongExpirationTimeAttribute());

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
                    Queues = new[] { "a_zatca","a_default", "b_nphies", "c_sms", "d_email", "e_daily", "f_clean_up", "g_noshow" }
                };

                _backgroundJobServer = new BackgroundJobServer(options);
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                if (_backgroundJobServer != null)
                {
                    _backgroundJobServer.Dispose();
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }

        void IRegisteredObject.Stop(bool immediate)
        {
            Stop();
        }
    }
}