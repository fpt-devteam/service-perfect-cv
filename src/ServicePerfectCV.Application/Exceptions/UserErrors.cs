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

        public static readonly Error PasswordInvalid = new(
            Code: "PasswordInvalid",
            Message: "Invalid password.",
            HttpStatusCode.BadRequest);
    }
}