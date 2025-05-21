using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Exceptions
{
    public class ConflictException<T> : BaseException
    {
        public ConflictException(string message)
            : base($"{typeof(T).Name}: {message}", HttpStatusCode.Conflict) { }
    }
}
