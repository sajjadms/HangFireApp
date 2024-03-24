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
        [Queue("sms")]
        public void ProcessSMSJobUsingHangfire()
        {
            var smsJobService = new SmsJobService();
            smsJobService.SendPendingSms();
        }

        [Queue("zatca")]
        public void ProcessZatcaJobUsingHangfire()
        {
            var zatcaReportingService = new ZatcaReportingService();
            zatcaReportingService.ReportPendingInvoices();
        }

        
        public void ScheduleRecurringJob()
        {
            // Schedule the ProcessSMSJobUsingHangfire method to run every 1 minutes
            string smsCronExpression = "*/1 * * * *"; // Runs every 1 minutes
            RecurringJob.AddOrUpdate("process-sms-job", () => ProcessSMSJobUsingHangfire(), smsCronExpression);

            string zatcaCronExpression = "*/1 * * * *"; // Runs every 1 minutes
            RecurringJob.AddOrUpdate("process-zatca-job", () => ProcessZatcaJobUsingHangfire(), zatcaCronExpression);
        }
    }
}