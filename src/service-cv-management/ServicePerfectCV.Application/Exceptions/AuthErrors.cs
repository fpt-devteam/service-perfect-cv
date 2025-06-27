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

        public static readonly Error InvalidActivationToken = new(
            Code: "InvalidToken",
            Message: "The activation token is invalid or expired.",
            HttpStatusCode.Forbidden);

        public static readonly Error OAuthTokenExchangeFailed = new(
            Code: "OAuthTokenExchangeFailed",
            Message: "Failed to exchange OAuth code for access token.",
            HttpStatusCode.BadRequest);

        public static readonly Error OAuthUserInfoFailed = new(
            Code: "OAuthUserInfoFailed",
            Message: "Failed to retrieve user information from OAuth provider.",
            HttpStatusCode.BadRequest);

        public static readonly Error GoogleOAuthConfigurationMissing = new(
            Code: "GoogleOAuthConfigurationMissing",
            Message: "Google OAuth configuration is missing or invalid.",
            HttpStatusCode.InternalServerError);
        public static readonly Error NotSupportedException = new(
            Code: "NotSupportedException",
            Message: "OAuth provider is not supported.",
            HttpStatusCode.BadRequest);

    }
}