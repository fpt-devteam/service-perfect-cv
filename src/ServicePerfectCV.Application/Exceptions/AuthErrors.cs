using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class AuthErrors
    {
        public static readonly Error PasswordInvalid = new(
            Code: "PasswordInvalid",
            Message: "Invalid password.",
            HttpStatusCode.BadRequest);

        public static readonly Error RefreshTokenInvalid = new(
            Code: "RefreshTokenInvalid",
            Message: "Invalid refresh token.",
            HttpStatusCode.Unauthorized);

        public static readonly Error UserNotAuthenticated = new(
            Code: "UserNotAuthenticated",
            Message: "User is not authenticated.",
            HttpStatusCode.Unauthorized);

        public static readonly Error Forbidden = new(
        Code: "Forbidden",
        Message: "You do not have permission to access this resource.",
        HttpStatusCode.Forbidden);
    }
}