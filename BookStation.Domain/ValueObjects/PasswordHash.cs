using BookStation.Core.SharedKernel;
using System.Text.RegularExpressions;

namespace BookStation.Domain.ValueObjects;

public sealed partial class PasswordHash : ValueObject
{
    private const int MinBcryptLength = 59;

    public string HashedValue { get; private set; }

    // Required for EF Core
    private PasswordHash()
    {
        HashedValue = string.Empty;
    }

    private PasswordHash(string hashedValue)
    {
        HashedValue = hashedValue;
    }

    public static PasswordHash FromHash(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new ArgumentException("Password hash cannot be empty.", nameof(hashedValue));

        if (!IsValidBcryptHash(hashedValue))
            throw new ArgumentException("Invalid BCrypt password hash format.", nameof(hashedValue));

        return new PasswordHash(hashedValue);
    }

    internal static PasswordHash FromPersistence(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new ArgumentException("Password hash cannot be empty.", nameof(hashedValue));

        return new PasswordHash(hashedValue);
    }

    private static bool IsValidBcryptHash(string value)
    {
        if (value.Length < MinBcryptLength)
            return false;

        return BcryptHashRegex().IsMatch(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedValue;
    }

    [GeneratedRegex(@"^\$2[aby]?\$\d{2}\$[./A-Za-z0-9]{53}$", RegexOptions.Compiled)]
    private static partial Regex BcryptHashRegex();
}


// la lop bao ve cuoi cung truoc khi luu vao database