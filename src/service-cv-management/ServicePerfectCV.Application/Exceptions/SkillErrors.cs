using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class SkillErrors
    {
        public static readonly Error NotFound = new(
            Code: "SkillNotFound",
            Message: "Skill not found.",
            HttpStatusCode.NotFound);

        public static readonly Error CVNotFound = new(
            Code: "CVNotFound",
            Message: "CV not found.",
            HttpStatusCode.NotFound);

        public static readonly Error InvalidItems = new(
            Code: "InvalidSkillItems",
            Message: "Invalid skill items provided.",
            HttpStatusCode.BadRequest);
        public static readonly Error CategoryNotProvided = new(
            Code: "CategoryNotProvided",
            Message: "Category not provided.",
            HttpStatusCode.BadRequest);
    }
}
