using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class SummaryErrors
    {
        public static readonly Error NotFound = new(
            Code: "SummaryNotFound",
            Message: "Summary not found.",
            HttpStatusCode.NotFound);

        public static readonly Error CVNotFound = new(
            Code: "CVNotFound",
            Message: "CV not found.",
            HttpStatusCode.NotFound);

        public static readonly Error AlreadyExists = new(
            Code: "SummaryAlreadyExists",
            Message: "Summary for this CV already exists.",
            HttpStatusCode.Conflict);
    }
}