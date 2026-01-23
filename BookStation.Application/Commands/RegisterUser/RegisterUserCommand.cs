using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BookStation.Application.Commands.RegisterUser;

/// <summary>
///  Command to register a new user.
///  Implement IRequest<T> để MediatR có thể dispatch.
/// </summary>
public record RegisterUserCommand : IRequest<RegisterUserResult>, IBaseRequest, IEquatable<RegisterUserCommand>
{
    public string Email { get; init; }
    public string Passwords { get; init; }
    public string FullName { get; init; }
    public string? PhoneNumber { get; init; }
}

/// <summary>
///  Result of the RegisterUserCommand.
/// </summary>
public record RegisterUserResult(long UserId, string Email, bool IsVerified);