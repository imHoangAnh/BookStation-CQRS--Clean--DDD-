using System;
using System.Collections.Generic;
using System.Text;

using MediatR;
using BookStation.Application.Contracts;
using BookStation.Domain.Repositories;
using BookStation.Domain.ValueObjects;

namespace BookStation.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;


    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;

    }
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        var password = Password.Create(request.Password);
        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        //chek if user is active
        if (user.Status != Domain.Enums.UserStatus.Active)
        {
            throw new UnauthorizedAccessException("User account is not active.");
        }
        // Generate JWT token
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);

        return new LoginResult
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
        
    }
}
