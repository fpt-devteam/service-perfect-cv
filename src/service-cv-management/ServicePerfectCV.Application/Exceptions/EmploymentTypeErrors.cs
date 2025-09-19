using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class EmploymentTypeErrors
    {
        public static readonly Error NotFound = new(
            Code: "EmploymentTypeNotFound",
            Message: "Employment type not found",
            HttpStatusCode.NotFound);
    }
}