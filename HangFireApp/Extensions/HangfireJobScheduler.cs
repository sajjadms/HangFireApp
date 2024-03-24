using Hangfire;
using HangFireService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HangFireApp.Extensions
{
    public class HangfireJobScheduler
    {
        public void ProcessSMSJobUsingHangfire()
        {
            var smsJobService = new SmsJobService();
            smsJobService.SendPendingSms();
        }

        public void ScheduleRecurringJob()
        {
            string cronExpression = "*/1 * * * *"; // Runs every 1 minutes

            // Schedule the ProcessSMSJobUsingHangfire method to run every 10 minutes
            RecurringJob.AddOrUpdate("process-sms-job", () => ProcessSMSJobUsingHangfire(), cronExpression);
        }
    }
}