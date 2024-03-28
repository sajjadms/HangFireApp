using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage.Monitoring;
using Hangfire.Storage;
using HangFireService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HangFireApp.Extensions
{
    public class HangfireJobScheduler
    {
        [Queue("c_sms")]
        public void SMSJob()
        {
            var smsJobService = new SmsJobService();
            smsJobService.SendPendingSms();

            Console.WriteLine("SMS Job Executed ..");
        }

        [Queue("a_zatca")]
        public void ZatcaJob()
        {
            var zatcaReportingService = new ZatcaReportingService();
            zatcaReportingService.ReportPendingInvoices();

            Console.WriteLine("Zatca Job Executed ..");
        }

        [Queue("b_nphies")]
        public void NphiesPreAuthJob()
        {
            Console.WriteLine("NphiesPreAuthJob Executed ..");

            TestMethod();
        }

        [Queue("b_nphies")]
        public void NphiesClaimsJob()
        {
            Console.WriteLine("NphiesClaimsJob Executed ..");

            TestMethod();
        }

        [Queue("b_nphies")]
        public void NphiesReconciliationJob()
        {
            Console.WriteLine("NphiesReconciliationJob Executed ..");

            TestMethod();
        }

        [Queue("e_daily")]
        public void AssetDepreciationJob()
        {
            
        }

        [Queue("f_clean_up")]
        public void CleanupJob()
        {
            int expirationDays = 10; // 1 day
            DeleteSucceededJobs(expirationDays);
            DeleteFailedJobs(expirationDays);
            DeleteEnQueuedJobs(expirationDays);
        }

        public void DeleteSucceededJobs(int expirationDays)
        {
            try
            {

                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var succeededJobs = monitoringApi.SucceededJobs(0, int.MaxValue); // Get all succeeded jobs

                Console.WriteLine($"Deleted Succeded Job : {succeededJobs.Count}");

                foreach (var job in succeededJobs)
                {
                    var jobDetails = monitoringApi.JobDetails(job.Key);

                    if (jobDetails == null)
                    {
                        continue;
                    }

                    if (jobDetails.ExpireAt == null)
                    {
                        continue;
                    }

                    if (DateTime.UtcNow > jobDetails.ExpireAt.Value)
                    {
                        BackgroundJob.Delete(job.Key); // Delete the job
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteSucceededJobs: {ex.Message}");
            }

        }

        public void DeleteFailedJobs(int expirationDays)
        {
            try
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var failedJobs = monitoringApi.FailedJobs(0, int.MaxValue); // Get all failed jobs

                Console.WriteLine($"Deleted Succeded Job : {failedJobs.Count}");

                foreach (var job in failedJobs)
                {
                    var jobDetails = monitoringApi.JobDetails(job.Key);

                    if (jobDetails == null)
                    {
                        continue;
                    }

                    if (jobDetails.ExpireAt == null)
                    {
                        continue;
                    }

                    if (DateTime.UtcNow > jobDetails.ExpireAt.Value)
                    {
                        BackgroundJob.Delete(job.Key); // Delete the job
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteFailedJobs: {ex.Message}");
            }

        }

        public void DeleteEnQueuedJobs(int expirationDays)
        {
            try
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var queuedJobs = monitoringApi.EnqueuedJobs("default", 0, int.MaxValue); // Get all jobs in the "default" queue

                Console.WriteLine($"Deleted Succeded Job : {queuedJobs.Count}");

                foreach (var job in queuedJobs)
                {
                    var jobDetails = monitoringApi.JobDetails(job.Key);

                    if (jobDetails == null)
                    {
                        continue;
                    }

                    if (jobDetails.ExpireAt == null)
                    {
                        continue;
                    }

                    if (DateTime.UtcNow > jobDetails.ExpireAt.Value)
                    {
                        BackgroundJob.Delete(job.Key); // Delete the job
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in DeleteEnQueuedJobs: {ex.Message}");
            }

        }

        public void TestMethod()
        {

        }

        [Queue("g_noshow")]
        public void NoShowVisitsJob()
        {

        }

        public void ScheduleRecurringJob()
        {
            RecurringJob.AddOrUpdate("sms-job", () => SMSJob(), "*/2 * * * *"); // every 2 minute

            RecurringJob.AddOrUpdate("zatca-job", () => ZatcaJob(), "*/3 * * * *"); // every 3 minute

            RecurringJob.AddOrUpdate("nphies-preauth-job", () => NphiesPreAuthJob(), "*/5 * * * *"); // every 5 minutes

            RecurringJob.AddOrUpdate("nphies-claims-job", () => NphiesClaimsJob(), "*/5 * * * *"); // every 5 minutes

            RecurringJob.AddOrUpdate("nphies-reconciliation-job", () => NphiesReconciliationJob(), Cron.Daily); // Daily

            RecurringJob.AddOrUpdate("noshowvisit-job", () => AssetDepreciationJob(), "*/30 * * * *"); // every 30 minute

            // Schedule the job to run daily at 23:00 UTC ,which is 02:00 AM Arab Standard Time
            RecurringJob.AddOrUpdate("assets-job", () => NoShowVisitsJob(), "0 23 * * *");

            // Schedule cleanup job to run daily
            RecurringJob.AddOrUpdate("cleanup-job", () => CleanupJob(), Cron.Daily); // 12:00 AM

            
        }

        
    }
}