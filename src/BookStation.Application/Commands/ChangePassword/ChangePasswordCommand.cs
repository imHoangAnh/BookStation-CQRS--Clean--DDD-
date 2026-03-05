using System;
using MediatR;

namespace BookStation.Application.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<ChangePasswordResult>
{
    public required Guid UserId { get; init; }
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}
public record ChangePasswordResult
{
    public Guid UserId { get; init; }
    public DateTime UpdatedAt { get; init; }
}

