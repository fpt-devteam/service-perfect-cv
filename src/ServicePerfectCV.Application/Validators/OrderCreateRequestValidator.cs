using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ServicePerfectCV.Application.DTOs.Order.Requests;

namespace ServicePerfectCV.Application.Validators
{
    public class OrderCreateRequestValidator : AbstractValidator<OrderCreateRequest>
    {
        public OrderCreateRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().NotNull()
                .Must(items => items.Count > 0).WithMessage("At least one item is required.");
            RuleForEach(x => x.Items).ChildRules(child =>
            {
                child.RuleFor(i => i.ItemId)
                  .NotEmpty().WithMessage("Product ID is required.")
                .NotNull().WithMessage("Product ID cannot be null.")
                  .WithMessage("Product ID must be a valid GUID.");

                child.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            });
        }
    }
}


