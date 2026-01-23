using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BookStation.Application.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}
