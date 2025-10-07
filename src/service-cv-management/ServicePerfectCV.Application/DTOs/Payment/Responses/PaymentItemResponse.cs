using System;

namespace ServicePerfectCV.Application.DTOs.Payment.Responses
{
    public class PaymentItemResponse
    {
        public required string Name { get; init; }
        public required int Quantity { get; init; }
        public required int Price { get; init; }
        public int TotalPrice => Quantity * Price;
    }
}