//using System;
//using System.Collections.Generic;
//using System.Text;
//using FluentValidation;

//namespace BookStation.Application.Commands.RegisterUser;

//public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
//{
//    public RegisterCommandValidator()
//    {
//        //Email
//        RuleFor(x => x.Email)
//            .NotEmpty().WithMessage("Email is required.")
//            .EmailAddress().WithMessage("Invalid email format.");
//        //Password
//        RuleFor(x => x.Password)
//            .NotEmpty().WithMessage("Password is required.")
//            .Custom((password, context) =>
//            {
//                if (string.IsNullOrEmpty(password))
//            });
//        RuleFor(x => x.FullName)
//            .NotEmpty().WithMessage("Full name is required.")
//            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");
//        RuleFor(x => x.PhoneNumber)
//            .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.")
//            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
//    }
//}
