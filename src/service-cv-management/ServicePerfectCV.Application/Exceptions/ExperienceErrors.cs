using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class ExperienceErrors
    {
        public static readonly Error NotFound = new(
            Code: "ExperienceNotFound",
            Message: "Experience not found.",
            HttpStatusCode.NotFound);

        public static readonly Error CVNotFound = new(
            Code: "CVNotFound",
            Message: "CV not found.",
            HttpStatusCode.NotFound);

        public static readonly Error InvalidRequest = new(
            Code: "InvalidExperienceRequest",
            Message: "Invalid experience request data.",
            HttpStatusCode.BadRequest);

        public static readonly Error Unauthorized = new(
            Code: "ExperienceUnauthorized",
            Message: "You are not authorized to access this experience.",
            HttpStatusCode.Forbidden);
    }
}