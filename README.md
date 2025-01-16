# NomSol.Hangfire.JobGuardian
JobGuardian for Hangfire is a custom job filter that prevents duplicate jobs from being enqueued by checking for existing jobs with the same name in the processing queue. It also tags duplicates with a custom label if the FaceIT.Hangfire.Tags library is installed, ensuring efficient job management and preventing redundant executions.

## PreventDuplicateJobAttribute for Hangfire
PreventDuplicateJobAttribute is a custom job filter for Hangfire that ensures no duplicate jobs are enqueued based on job name. If a job with the same name is already being processed, it prevents the new job from being queued and optionally tags the duplicate job if FaceIT.Hangfire.Tags is installed.

### Installation
1. Install the package: This library can be added to your project as a NuGet package.

```bash
dotnet add package NomSol.Hangfire.JobGuardian
```

### Usage
To use the PreventDuplicateJobAttribute, simply decorate your job method with the attribute.

``` csharp
public class MyJob
{
    [PreventDuplicateJob]
    public void ExecuteJob(string jobName)
    {
        // Your job logic here
    }
}
```

### How It Works

Duplicate Detection: Before a job is enqueued, the filter checks if a job with the same name is already in the processing state. If it finds a duplicate, it prevents the new job from being added to the queue.
Retry Handling: The filter will not check for duplicates if the job is being retried or is already in the "Processing" state.

Tagging Duplicates: If the FaceIT.Hangfire.Tags package is installed, the duplicate job will be tagged as "DeletedByDuplicateCheck" for easy tracking and management in the Hangfire dashboard.

Example:
``` csharp
public class JobService
{
    [PreventDuplicateJob]
    public void ProcessJob(string jobName)
    {
        // Job logic goes here
    }
}
```

### Key Features
Duplicate Prevention: Ensures that jobs with the same name do not get processed concurrently.
Hangfire Tags Integration: Supports tagging duplicate jobs for easy tracking if the FaceIT.Hangfire.Tags library is installed.

State Handling: Prevents job from being queued if it's a retry attempt or already in the "Processing" state.
Customizable Job Name: Allows flexible extraction of the job name from the job's arguments for detecting duplicates.

### Dependencies
- Hangfire (version >= 1.7.0)
- FaceIT.Hangfire.Tags (optional, for job tagging)