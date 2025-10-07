using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Payment.Requests
{
    public class CreatePaymentRequest
    {
        // /// <summary>
        // /// Order code (will be auto-generated if not provided)
        // /// </summary>
        // public int? OrderCode { get; set; }

        /// <summary>
        /// Package ID to purchase - Required for getting exact package data from database
        /// </summary>
        [Required(ErrorMessage = "PackageId is required")]
        public Guid PackageId { get; set; }

        // [Required]
        // [MaxLength(25, ErrorMessage = "Description cannot exceed 25 characters")]
        // public string Description { get; set; } = default!;
    }

}
