using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BookStation.Application.Commands.RegisterUser;

/// <summary>
///  Command to register a new user.
///  Implement IRequest<T> để MediatR có thể dispatch.
/// </summary>
public record RegisterUserCommand : IRequest<RegisterUserResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
    public required string FullName { get; init; }
    public string? PhoneNumber { get; init; }
}

/*public record RegisterUserCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string? FullName = null,
    string? Phone = null ) : IRequest<RegisterUserResult>;   --> nhuoc diem: de truyen sai thu tu new RegisterUserCommand(password,email,confirm,name),
                                                                 kho mo rong, them field moi phai sua lai constructor);
*/


/// <summary>
///  Result of the RegisterUserCommand.
/// </summary>
public record RegisterUserResult(long UserId, string Email, bool IsVerified)
{
};


/*
Khi bạn dùng record, C# compiler tự động generate:

public record RegisterUserCommand : IRequest<RegisterUserResult>, IEquatable<RegisterUserCommand>
{
    // Properties...

    // Tự động generate bởi compiler:
    public virtual bool Equals(RegisterUserCommand? other)
    {
        return other != null
            && Email == other.Email
            && Password == other.Password
            && FullName == other.FullName
            && PhoneNumber == other.PhoneNumber;
    }

    public override bool Equals(object? obj)
        => Equals(obj as RegisterUserCommand);

    public override int GetHashCode()
        => HashCode.Combine(Email, Password, FullName, PhoneNumber);

    public static bool operator ==(RegisterUserCommand? left, RegisterUserCommand? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(RegisterUserCommand? left, RegisterUserCommand? right)
        => !(left == right);
}
*/