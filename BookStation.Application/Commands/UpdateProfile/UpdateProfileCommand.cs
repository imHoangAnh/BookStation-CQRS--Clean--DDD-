using System;
using BookStation.Domain.Enums;
using MediatR;

namespace BookStation.Application.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest<UpdateProfileResult>
{
    public Guid UserId { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public Gender? Gender { get; init; }
    public string? Bio { get; init; }

    public UpdateProfileCommand(
        Guid userId, 
        string? fullName, 
        string? phoneNumber,
        DateTime? dateOfBirth = null,
        Gender? gender = null,
        string? bio = null)
    {
        UserId = userId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Bio = bio;
    }
}

public record UpdateProfileResult
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public Gender? Gender { get; init; }
    public string? Bio { get; init; }
    public DateTime UpdatedAt { get; init; }
}
