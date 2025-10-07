using ServicePerfectCV.Domain.Enums;
using System;

namespace ServicePerfectCV.Application.DTOs.Billing.Requests
{
    public class UpdateBillingHistoryRequest
    {
   
           public required PaymentStatus Status { get; set; }
           
    }
}