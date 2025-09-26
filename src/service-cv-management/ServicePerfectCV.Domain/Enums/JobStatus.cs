using System;

namespace ServicePerfectCV.Domain.Enums
{
    public enum JobStatus
    {
        Queued,
        Running,
        Succeeded,
        Failed,
        Canceled
    }
}
