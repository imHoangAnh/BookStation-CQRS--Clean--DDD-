using System;
using System.Collections.Generic;
using System.Text;
using BookStation.Core.SharedKernel;
using System.Text.RegularExpressions;

namespace BookStation.Domain.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    /// <summary>
    /// Expected minimum length for a valid password hash (Base64 encoded).
    /// Salt (16 bytes) + Hash (32 bytes) = 48 bytes -> ~64 chars in Base64.
    /// </summary>
    private const int HashLength = 40; 
    public string HashedValue { get; }

    private PasswordHash(string hashedValue)
    {
        HashedValue = hashedValue;
    }
    public static PasswordHash FromHash(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new ArgumentException("Password hash cannot be empty.", nameof(hashedValue));

        if (hashedValue.Length < HashLength)
            throw new ArgumentException("Invalid password hash format.", nameof(hashedValue));

        // Validate Base64 format
        if (!IsValidBase64(hashedValue))
            throw new ArgumentException("Password hash must be in valid Base64 format.", nameof(hashedValue));

        return new PasswordHash(hashedValue);
    }

    /// <summary>
    /// Creates a PasswordHash from database persistence 
    /// This method is slightly less strict for backward compatibility
    /// </summary>
    /// <returns>A new PasswordHash value object.</returns>
    internal static PasswordHash FromPersistence(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new ArgumentException("Password hash cannot be empty.", nameof(hashedValue));

        return new PasswordHash(hashedValue);
    }

    private static bool IsValidBase64(string value)
    {
        try
        {
            Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedValue;
    }

    public override string ToString() => "[PROTECTED]"; // Never expose hash in logs

    public static implicit operator string(PasswordHash hash) => hash.HashedValue;
}
