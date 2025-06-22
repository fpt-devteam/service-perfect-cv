using ServicePerfectCV.Application.Exceptions;
using System.Net;

namespace ServicePerfectCV.Application.Common
{
    public static class OrganizationErrors
    {
        public static readonly Error NotFound = new(
            Code: "OrganizationNotFound",
            Message: "Organization not found",
            HttpStatusCode: HttpStatusCode.NotFound);
            
        public static readonly Error AlreadyExists = new(
            Code: "OrganizationAlreadyExists",
            Message: "Organization with the same name already exists",
            HttpStatusCode: HttpStatusCode.Conflict);
    }
}
