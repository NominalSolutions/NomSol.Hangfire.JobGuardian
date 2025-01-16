using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using NomSol.Hangfire.JobGuard.Attributes.Helpers;
using System;
using System.Linq;

namespace NomSol.Hangfire.JobGuard.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PreventDuplicateJobAttribute : JobFilterAttribute, IElectStateFilter
    {
        public void OnStateElection(ElectStateContext context)
        {
            // Check if this is a retry attempt
            var retryCount = context.GetJobParameter<int>("RetryCount");
            string stateCheck = context.CurrentState;

            if (retryCount > 0 || stateCheck == "Processing")
            {
                return; // Skip duplicate check for retries
            }

            // Get the current job's name
            var jobName = context.BackgroundJob.Job.Args[2]?.ToString(); // Assuming the #2 argument is the job name
            if (string.IsNullOrEmpty(jobName))
            {
                return;
            }

            // Check if a job with the same name is already in the queue
            using (var connection = JobStorage.Current.GetConnection())
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var inQ = monitoringApi.ProcessingJobs(0, int.MaxValue);

                bool isDuplicate = inQ.Any(job =>
                    job.Value.Job.Args[2]?.ToString() == jobName); // Compare job names

                if (isDuplicate)
                {
                    if (Helpers.Helpers.IsHangfireTagsInstalled())
                    {
                        string tag = "DeletedByJobGuard";

                        using (var tran = context.Connection.CreateWriteTransaction())
                        {
                            if (!(tran is JobStorageTransaction))
                                throw new NotSupportedException(" Storage transactions must implement JobStorageTransaction");


                            var score = DateTime.Now.Ticks;

                            tran.AddToSet("tags", tag, score);
                            tran.AddToSet(context.BackgroundJob.Id.GetSetKey(), tag, score);
                            tran.AddToSet(tag.GetSetKey(), context.BackgroundJob.Id, score);

                            tran.Commit();
                        }
                    }

                    // Prevent the job from being queued
                    context.CandidateState = new DeletedState
                    {
                        Reason = $"Duplicate job detected with name: {jobName}"
                    };
                }
            }
        }
    }
}
