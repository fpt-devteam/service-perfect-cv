using System;

namespace ServicePerfectCV.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Canceled,
        Refunded,
        Expired
    }
}