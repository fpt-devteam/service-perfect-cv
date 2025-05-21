using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Exceptions
{
    public class BadRequestException<T> : BaseException
    {
        public BadRequestException(string message)
            : base($"{typeof(T).Name}: {message}", HttpStatusCode.BadRequest) { }
    }
}