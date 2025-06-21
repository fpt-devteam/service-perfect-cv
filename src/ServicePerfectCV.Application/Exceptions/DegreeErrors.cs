using System.Net;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class DegreeErrors
    {
        public static readonly Error NotFound = new Error(
            Code: "DegreeNotFound",
            Message: "Degree not found",
            HttpStatusCode: HttpStatusCode.NotFound);

        public static readonly Error AlreadyExists = new Error(
            Code: "DegreeAlreadyExists",
            Message: "Degree already exists",
            HttpStatusCode: HttpStatusCode.Conflict);
    }
}
