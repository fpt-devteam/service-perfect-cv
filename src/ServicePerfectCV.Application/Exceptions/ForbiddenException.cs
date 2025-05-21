using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Exceptions
{
    public class ForbiddenException<T> : BaseException
    {
        public ForbiddenException(string message = "You do not have permission to access this resource.")
            : base($"{typeof(T).Name}: {message}", HttpStatusCode.Forbidden) { }
    }
}
