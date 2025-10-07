using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class PackageErrors
    {
        public static readonly Error NotFound = new(
            Code: "PackageNotFound",
            Message: "Package not found.",
            HttpStatusCode.NotFound);

        public static readonly Error NameAlreadyExists = new(
            Code: "PackageNameAlreadyExists",
            Message: "A package with this name already exists.",
            HttpStatusCode.Conflict);

        public static readonly Error InvalidPrice = new(
            Code: "InvalidPackagePrice",
            Message: "Package price must be greater than 0.",
            HttpStatusCode.BadRequest);

        public static readonly Error InvalidCredits = new(
            Code: "InvalidPackageCredits",
            Message: "Package credits must be at least 1.",
            HttpStatusCode.BadRequest);

        public static readonly Error CannotDeletePackageWithPurchases = new(
            Code: "CannotDeletePackageWithPurchases",
            Message: "Cannot delete package that has been purchased by users.",
            HttpStatusCode.Conflict);

        public static readonly Error AlreadyActive = new(
            Code: "PackageAlreadyActive",
            Message: "Package is already active.",
            HttpStatusCode.BadRequest);

        public static readonly Error AlreadyInactive = new(
            Code: "PackageAlreadyInactive",
            Message: "Package is already inactive.",
            HttpStatusCode.BadRequest);
    }
}