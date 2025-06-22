using System;
using System.Net;

namespace ServicePerfectCV.Application.Exceptions
{
    public static class CertificationErrors
    {
        public static readonly Error NotFound = new Error    
        (
            Code: "Certification_NotFound",
            Message: "Certification not found",
            HttpStatusCode: HttpStatusCode.NotFound
        );

        public static readonly Error OrganizationNotFound = new Error
        (
            Code: "Certification_Organization_NotFound",
            Message: "Organization not found",
            HttpStatusCode: HttpStatusCode.NotFound
        );

        public static readonly Error CVNotFound = new Error
        (
            Code: "Certification_CV_NotFound",
            Message: "CV not found",
            HttpStatusCode: HttpStatusCode.NotFound
        );
    }
}