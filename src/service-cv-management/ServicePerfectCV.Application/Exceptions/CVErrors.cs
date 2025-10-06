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

        public static readonly Error JobDescriptionNotFound = new(
            Code: "JobDescriptionNotFound",
            Message: "Job description not found.",
            HttpStatusCode.NotFound);

        public static readonly Error JobDescriptionValidationFailed = new(
            Code: "JobDescriptionValidationFailed",
            Message: "Job description validation failed.",
            HttpStatusCode.BadRequest);

        public static readonly Error JobDescriptionAlreadyExists = new(
            Code: "JobDescriptionAlreadyExists",
            Message: "A job description already exists for this CV.",
            HttpStatusCode.Conflict);

        public static readonly Error JobDescriptionCreationFailed = new(
            Code: "JobDescriptionCreationFailed",
            Message: "Failed to create job description.",
            HttpStatusCode.InternalServerError);

        public static readonly Error JobDescriptionUpdateFailed = new(
            Code: "JobDescriptionUpdateFailed",
            Message: "Failed to update job description.",
            HttpStatusCode.InternalServerError);

        public static readonly Error InvalidJobDescriptionId = new(
            Code: "InvalidJobDescriptionId",
            Message: "Invalid job description ID provided.",
            HttpStatusCode.BadRequest);

        public static readonly Error ExceedMaxAllowedSize = new(
            Code: "ExceedMaxAllowedSize",
            Message: "The uploaded file exceeds the maximum allowed size (10MB).",
            HttpStatusCode.BadRequest);

        public static readonly Error InvalidFileType = new(
            Code: "InvalidFileType",
            Message: "Invalid file type. Only PDF files are allowed.",
            HttpStatusCode.BadRequest);
    }
}