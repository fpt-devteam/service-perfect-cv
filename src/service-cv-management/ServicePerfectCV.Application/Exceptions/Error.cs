using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public sealed record Error(string Code, string Message, HttpStatusCode HttpStatusCode);
}