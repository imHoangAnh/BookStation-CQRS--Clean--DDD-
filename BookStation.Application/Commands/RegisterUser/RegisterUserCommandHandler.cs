using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using BookStation.Domain.ValueObjects;
using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Repositories;

namespace BookStation.Application.Commands.RegisterUser;
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly I
}

