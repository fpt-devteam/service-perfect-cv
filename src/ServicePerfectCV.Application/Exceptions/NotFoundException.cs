using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Exceptions
{
    public class NotFoundException<T> : BaseException
    {
        public NotFoundException()
            : base($"{typeof(T).Name}  was not found.", HttpStatusCode.NotFound) { }
    }
}