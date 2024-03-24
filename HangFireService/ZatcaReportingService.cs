using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangFireService
{
    public class ZatcaReportingService
    {
        public ZatcaReportingService()
        {             // This is a constructor
        }

        public void ReportPendingInvoices()
        {
            // This method will be called by Hangfire
            // to report pending invoices to ZATCA
        }
    }
}
