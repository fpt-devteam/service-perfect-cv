using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Exceptions
{
    public class UnauthorizedException<T> : BaseException
    {
        public UnauthorizedException(string message = "Authentication is required to access this resource.")
            : base($"{typeof(T).Name}: {message}", HttpStatusCode.Unauthorized) { }
    }
}
