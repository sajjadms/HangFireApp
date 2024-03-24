using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HangFireApp.Extensions
{
    public class LogFailureAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var backgroundJob = context.BackgroundJob;
            var jobId = backgroundJob.Id;

            if (context.NewState is FailedState failedState)
            {
                Logger.ErrorException(
                $"Background job #{jobId} was failed with an exception.", failedState.Exception);
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}