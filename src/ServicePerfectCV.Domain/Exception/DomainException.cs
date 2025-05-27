using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Exception
{
    public class DomainException(string message) : System.Exception(message)
    {
    }
}