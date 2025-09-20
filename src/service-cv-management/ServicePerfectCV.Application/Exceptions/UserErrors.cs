using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class UserErrors
    {
        public static readonly Error NotFound = new(
            Code: "UserNotFound",
            Message: "User not found.",
            HttpStatusCode.NotFound);
        public static readonly Error EmailAlreadyExists = new(
            Code: "EmailAlreadyExists",
            Message: "Email already exists.",
            HttpStatusCode.Conflict);
        public static readonly Error AccountNotActivated = new(
            Code: "AccountNotActivated",
            Message: "Account is existed but not activated.",
            HttpStatusCode.Forbidden);
        public static readonly Error AccountAlreadyActivated = new(
            Code: "AccountAlreadyActivated",
            Message: "Account is already activated.",
            HttpStatusCode.Forbidden);

        public static readonly Error NoFileUploaded = new(
            Code: "NoFileUploaded",
            Message: "No file uploaded.",
            HttpStatusCode.BadRequest);

        public static readonly Error InvalidUserId = new(
            Code: "InvalidUserId",
            Message: "Invalid user identifier format.",
            HttpStatusCode.BadRequest);
    }
}