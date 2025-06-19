using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class CompanyErrors
    {
        public static readonly Error NotFound = new(
            Code: "CompanyNotFound",
            Message: "Company not found",
            HttpStatusCode.NotFound);
    }
}
