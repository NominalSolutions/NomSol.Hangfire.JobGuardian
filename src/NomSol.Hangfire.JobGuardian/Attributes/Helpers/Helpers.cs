using System;
using System.Linq;

namespace NomSol.Hangfire.JobGuard.Attributes.Helpers
{
    internal static class Helpers
    {
        internal const string SetKey = "tags";

        public static string GetSetKey(this string jobId)
        {
            return $"{SetKey}:{jobId}";
        }

        public static bool IsHangfireTagsInstalled()
        {
            // Check loaded assemblies for the specific assembly or type
            var isTagsInstalled = AppDomain.CurrentDomain.GetAssemblies()
                .Any(assembly => assembly.FullName.Contains("FaceIT.Hangfire.Tags"));

            return isTagsInstalled;
        }
    }
}
