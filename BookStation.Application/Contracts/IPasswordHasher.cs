using BookStation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Application.Contracts;

public interface IPasswordHasher
{
    PasswordHash HashPassword(Password password);
    bool VerifyPassword(Password password, PasswordHash passwordHash);
}