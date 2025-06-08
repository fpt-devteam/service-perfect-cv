using FluentValidation;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Validators
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator() 
        {
            RuleFor(x => x.Email).
                NotEmpty().WithMessage("Email is required.").
                EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
