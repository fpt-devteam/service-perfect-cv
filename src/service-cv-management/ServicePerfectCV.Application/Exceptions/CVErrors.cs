using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class CVErrors
    {
        public static readonly Error CVNotFound = new(
            Code: "CVNotFound",
            Message: "CV not found.",
            HttpStatusCode.NotFound);

        public static readonly Error Unauthorized = new(
            Code: "CVUnauthorized",
            Message: "You are not authorized to access this CV.",
            HttpStatusCode.Unauthorized);

        public static readonly Error ValidationFailed = new(
            Code: "CVValidationFailed",
            Message: "CV validation failed.",
            HttpStatusCode.BadRequest);

        public static readonly Error CreationFailed = new(
            Code: "CVCreationFailed",
            Message: "Failed to create CV.",
            HttpStatusCode.InternalServerError);

        public static readonly Error UpdateFailed = new(
            Code: "CVUpdateFailed",
            Message: "Failed to update CV.",
            HttpStatusCode.InternalServerError);

        public static readonly Error DeleteFailed = new(
            Code: "CVDeleteFailed",
            Message: "Failed to delete CV.",
            HttpStatusCode.InternalServerError);
    }
}