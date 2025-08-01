using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class ContactErrors
    {
        public static readonly Error NotFound = new(
            Code: "ContactNotFound",
            Message: "Contact not found.",
            HttpStatusCode.NotFound);
            
        public static readonly Error CVNotFound = new(
            Code: "CVNotFound",
            Message: "CV not found.",
            HttpStatusCode.NotFound);
            
        public static readonly Error AlreadyExists = new(
            Code: "ContactAlreadyExists",
            Message: "Contact for this CV already exists.",
            HttpStatusCode.Conflict);
    }
}
