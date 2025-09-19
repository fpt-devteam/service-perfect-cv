using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class EducationErrors
    {
        public static readonly Error NotFound = new(
            Code: "EDU001",
            Message: "Education not found",
            HttpStatusCode.NotFound);

        public static readonly Error CVNotFound = new(
            Code: "EDU002",
            Message: "CV not found",
            HttpStatusCode.NotFound);

        public static readonly Error DegreeNotFound = new(
            Code: "EDU003",
            Message: "Degree not found",
            HttpStatusCode.NotFound);

        public static readonly Error OrganizationNotFound = new(
            Code: "EDU004",
            Message: "Organization not found",
            HttpStatusCode.NotFound);
    }
}