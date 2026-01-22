using BookStation.Core.SharedKernel;
using System.Text.RegularExpressions;

namespace BookStation.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    private const int MaxLength = 255;

    public string Value { get; }



    /// Private constructor - using factory method
    /* Factory method cho phep tao doi tuong thong qua mot phuong thuc trung gian thay vi truc tiep su dung constructor
     * Thay the cho viec goi new ClassName() trong code, giup code linh hoat va de mo rong hon 
     * So sanh voi new() - phu thuoc class cu the, kho mo rong, logic tao object rai rac, kho test
     * factory method - phu thuoc abstraction, de mo rong, logic tao object tap trung, de mock/test
     * 
     */
    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Factory method to create a new Email value object.   
    /// </summary>
    /// <param name="email">The email address string.</param>
    /// <returns>A new Email value object.</returns>
    /// <exception cref="ArgumentException">Thrown when the email is invalid.</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (email.Length > MaxLength)
            throw new ArgumentException($"Email cannot exceed {MaxLength} characters.", nameof(email));

        if (!EmailRegex().IsMatch(email))
            throw new ArgumentException("Email format is invalid.", nameof(email));

        return new Email(email.ToLowerInvariant().Trim());
    }

    /// <summary>
    /// Tries to create a new Email value object.
    /// </summary>
    public static bool TryCreate(string email, out Email? result)
    {
        try
        {
            result = Create(email);
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

    public static implicit operator string(Email email) => email.Value; // chuyen kieu tu dong 

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}
    