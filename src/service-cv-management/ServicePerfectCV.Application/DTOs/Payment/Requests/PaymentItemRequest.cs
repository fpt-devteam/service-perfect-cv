using System;
using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.Application.DTOs.Payment.Requests
{
    public class PaymentItemRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Item name cannot exceed 50 characters")]
        public string Name { get; set; } = default!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be at least 1")]
        public int Price { get; set; }
    }
}