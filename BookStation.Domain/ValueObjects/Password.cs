using System;
using System.Collections.Generic;
using System.Text;
using BookStation.Core.SharedKernel;
using System.Text.RegularExpressions;

namespace BookStation.Domain.ValueObjects;

public sealed partial class Password : ValueObject
{
    public const int MinLength = 8;
    public string Value { get; }
    private Password(string value)
    {
        Value = value;
    }
    public static Password Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }
        if (password.Length < MinLength)
        {
            throw new ArgumentException($"Password must be at least {MinLength} characters long.", nameof(password));
        }
        if (!HasUpperCase(password))
            throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(password));
        if (!HasSpecialCharacter(password))
            throw new ArgumentException("Password must contain at least one special character.", nameof(password));
        return new Password(password);
    }

    /// <summary>
    /// Tries to create a new Password value object.
    /// </summary>
    public static bool TryCreate(string password, out Password? result, out string? errorMessage)
    {
        try
        {
            result = Create(password);
            errorMessage = null;
            return true;    
        }
        catch (ArgumentException ex)
        {
            result = null;
            errorMessage = ex.Message;
            return false;
        }
    }
    /// <summary>
    /// Validates a password string without creating an instance.
    /// </summary>
    public static (bool IsValid, List<string> Errors) Validate(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password cannot be empty.");
            return (false, errors);
        }
        if (password.Length < MinLength)
            errors.Add($"Password must be at least {MinLength} characters.");
 
        if (!HasUpperCase(password))
            errors.Add("Password must contain at least one uppercase letter.");

        if (!HasSpecialCharacter(password))
            errors.Add("Password must contain at least one special character.");

        return (errors.Count == 0, errors);
    }

    private static bool HasUpperCase(string password) =>
           UpperCaseRegex().IsMatch(password);

    private static bool HasSpecialCharacter(string password) =>
        SpecialCharacterRegex().IsMatch(password);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => "********"; // Never show password in logs

    public static implicit operator string(Password password) => password.Value;

    [GeneratedRegex(@"[A-Z]", RegexOptions.Compiled)]
    private static partial Regex UpperCaseRegex();

    [GeneratedRegex(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?~`]", RegexOptions.Compiled)]
    private static partial Regex SpecialCharacterRegex();
}