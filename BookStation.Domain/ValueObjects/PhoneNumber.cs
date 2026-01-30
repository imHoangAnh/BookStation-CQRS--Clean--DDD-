using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BookStation.Core.SharedKernel;

namespace BookStation.Domain.ValueObjects;

public sealed partial class PhoneNumber : ValueObject
{
    private const int PhoneLength = 10;

    /// <summary>
    /// Gets the phone number value.
    /// </summary>
    public string Value { get; private set; }

    // Required for EF Core
    private PhoneNumber()
    {
        Value = string.Empty;
    }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new PhoneNumber value object.
    /// </summary>
    public static PhoneNumber Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone number cannot be empty.", nameof(phone));

        // Remove spaces and dashes
        var cleaned = phone.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("(", "").Replace(")", "");

        if (cleaned.StartsWith("+84"))
            cleaned = "0" + cleaned.Substring(3);

        if (cleaned.StartsWith("84") && cleaned.Length >= 11)
            cleaned = "0" + cleaned.Substring(2);

        if (cleaned.Length != PhoneLength)
            throw new ArgumentException($"Phone number must have {PhoneLength} digits.", nameof(phone));

        if (!PhoneRegex().IsMatch(cleaned))
            throw new ArgumentException("Phone number format is invalid.", nameof(phone));

        return new PhoneNumber(cleaned);
    }

    /// <summary>
    /// Tries to create a new PhoneNumber value object.
    /// </summary>
    public static bool TryCreate(string phone, out PhoneNumber? result)
    {
        try
        {
            result = Create(phone);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;

    [GeneratedRegex(@"^0\d{9}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();
}

