using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class BillingErrors
    {
        public static readonly Error NotFound = new(
            Code: "BillingHistoryNotFound",
            Message: "Billing history not found.",
            HttpStatusCode.NotFound);
    }
}
