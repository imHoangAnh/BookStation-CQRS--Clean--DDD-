using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using BookStation.Domain.ValueObjects;
using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Repositories;
using BookStation.Application.Contracts;

namespace BookStation.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
        }
        // Create Email value object
        var email = Email.Create(request.Email);

        // Create Password value object
        var password = Password.Create(request.Password);

        // Hash the password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create PhoneNumber value object if provided
        var phoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : PhoneNumber.Create(request.PhoneNumber);

        // Create User entity
        var user = User.Create(email, passwordHash, request.FullName, phoneNumber);

        // Save user to repository
        await _userRepository.TaskAsync(user, cancellationToken);
        // Return result
        return new RegisterUserResult(user.Id, user.Email.Value, user.IsVerified);

    }
}           

