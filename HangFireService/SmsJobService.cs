using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangFireService
{
    public class SmsJobService
    {
        public SmsJobService() { }

        public void SendPendingSms()
        {
            // logic to fetch the sms which are pending
            // and send to sms provider api

            //throw new Exception("Failed to send sms");

            //send mail
            BackgroundJob.Enqueue(() => EmailService.SendEmail());
        }
    }
}
