using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Application.Contracts;

public interface IJwtTokenGenerator
{
    string GenerateToken(long userId, string email);
}