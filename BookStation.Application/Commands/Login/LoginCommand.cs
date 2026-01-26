using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BookStation.Application.Commands.Login;

public record LoginCommand : IRequest<LoginResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public record LoginResult
{
    public Guid UserId { get; init; }
    public required string Token { get; init; }
    public DateTime ExpiresAt { get; init; }
}
