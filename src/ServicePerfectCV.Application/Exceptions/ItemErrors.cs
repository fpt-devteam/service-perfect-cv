using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class ItemErrors
    {
        public static readonly Error NotFound = new(
            Code: "ItemNotFound",
            Message: "Item not found.",
            HttpStatusCode.NotFound);

        public static readonly Error InvalidItem = new(
            Code: "InvalidItem",
            Message: "Invalid item data.",
            HttpStatusCode.BadRequest);

        public static readonly Error DuplicateItem = new(
            Code: "DuplicateItem",
            Message: "Item already exists.",
            HttpStatusCode.Conflict);

        public static readonly Error ItemInUse = new(
            Code: "ItemInUse",
            Message: "Item is currently in use and cannot be deleted.",
            HttpStatusCode.Conflict);

        public static readonly Error ItemUpdateFailed = new(
            Code: "ItemUpdateFailed",
            Message: "Failed to update item.",
            HttpStatusCode.BadRequest);

        public static readonly Error InsufficientStock = new(
            Code: "InsufficientStock",
            Message: "Insufficient stock.",
            HttpStatusCode.BadRequest);
    }
}