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
            Message: "Account is not activated.",
            HttpStatusCode.Forbidden);
    }
}