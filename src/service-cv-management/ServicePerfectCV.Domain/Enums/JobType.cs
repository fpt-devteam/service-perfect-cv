using System;

namespace ServicePerfectCV.Domain.Enums
{
    public enum JobType
    {
        ScoreCV,
        // Add more job types here as needed
        // GenerateResume,
        // OptimizeCV,
        // etc.
    }

    public static class JobTypeExtensions
    {
        public static JobType FromString(string value)
        {
            if (Enum.TryParse<JobType>(value, true, out var result))
                return result;

            throw new ArgumentException($"Invalid job type: {value}");
        }
    }
}


