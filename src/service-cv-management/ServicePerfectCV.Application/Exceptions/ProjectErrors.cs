using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class ProjectErrors
    {
        public static Error NotFound => new Error("Project.NotFound", "Project not found", HttpStatusCode.NotFound);

        public static Error CVNotFound => new Error("Project.CVNotFound", "CV not found", HttpStatusCode.NotFound);
    }
}
