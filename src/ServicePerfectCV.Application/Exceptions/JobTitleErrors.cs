using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class JobTitleErrors
    {
        public static readonly Error NotFound = new(
            Code: "JobTitleNotFound",
            Message: "Job title not found",
            HttpStatusCode.NotFound);
    }
}
