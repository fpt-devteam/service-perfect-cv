using System;
using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Payment.Requests
{
    public class CancelPaymentRequest
    {
        [MaxLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters")]
        public string? CancellationReason { get; set; }
    }
}