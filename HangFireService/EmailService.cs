using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangFireService
{
    public class EmailService
    {
        [Queue("d_email")]
        public static void SendEmail()
        {

        }
    }
}
