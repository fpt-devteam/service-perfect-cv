using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class OrderErrors
    {
        public static readonly Error NotFound = new(
            Code: "OrderNotFound",
            Message: "Order not found.",
            HttpStatusCode.NotFound);

        public static readonly Error InvalidOrder = new(
            Code: "InvalidOrder",
            Message: "Invalid order.",
            HttpStatusCode.BadRequest);


    }
}