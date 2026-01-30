using BookStation.Application.Contracts;
using BookStation.Domain.ValueObjects;

namespace BookStation.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    // Work factor - higher = more secure but slower 
    private const int WorkFactor = 12;

    public PasswordHash HashPassword(Password password)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password.Value, WorkFactor);
        return PasswordHash.FromHash(hashedPassword);
    }

    public bool VerifyPassword(Password password, PasswordHash passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password.Value, passwordHash.HashedValue);
    }
}