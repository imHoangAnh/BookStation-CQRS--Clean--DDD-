//using BookStation.Core.SharedKernel;
//using System.Text.RegularExpressions;

//namespace BookStation.Domain.ValueObjects;

///// <summary>
///// Value object ISBN (International Standard Book Number).
///// Supports both ISBN-10 and ISBN-13 formats with validation.
///// Before 2007, ISBNs were 10 digits long. Since January 1, 2007, ISBNs have been 13 digits long.
///// </summary>
//public sealed partial class ISBN : ValueObject
//{
//    private const int ISBN10_LENGTH = 10;
//    private const int ISBN13_LENGTH = 13;

//    /// <summary>
//    /// Gets the normalized ISBN value (without hyphens or spaces).
//    /// </summary>
//    public string Value { get; }

//    /// <summary>
//    /// Gets the ISBN type (10 or 13 digits).
//    /// </summary>
//    public IsbnType Type { get; }

//    private string? _formattedCache;

//    private ISBN(string value, IsbnType type)
//    {
//        Value = value;
//        Type = type;
//    }

//    /// <summary>
//    /// Creates a new ISBN value object.
//    /// </summary>
//    /// <exception> ArgumentException --> Thrown when ISBN is invalid.</exception>
//    public static ISBN Create(string isbn)
//    {
//        if (string.IsNullOrWhiteSpace(isbn))
//            throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));

//        var cleaned = NormalizeIsbn(isbn);

//        if (TryValidateIsbn(cleaned, out var type))
//            return new ISBN(cleaned, type);

//        throw new ArgumentException($"ISBN format is invalid: {isbn}", nameof(isbn));
//    }

//    /// <summary>
//    /// Tries to create a new ISBN value object.
//    /// </summary>
//    public static bool TryCreate(string? isbn, out ISBN? result)
//    {
//        result = null;

//        if (string.IsNullOrWhiteSpace(isbn))
//            return false;

//        try
//        {
//            var cleaned = NormalizeIsbn(isbn);

//            if (TryValidateIsbn(cleaned, out var type))
//            {
//                result = new ISBN(cleaned, type);
//                return true;
//            }
//        }
//        catch
//        {
//            // Swallow exceptions for TryCreate pattern
//        }

//        return false;
//    }

//    private static string NormalizeIsbn(string isbn)
//    {
//        // Use Span<char> for better performance if dealing with many ISBNs
//        return isbn.Replace("-", "", StringComparison.Ordinal)
//                   .Replace(" ", "", StringComparison.Ordinal);
//    }

//    private static bool TryValidateIsbn(string isbn, out IsbnType type)
//    {
//        type = IsbnType.Unknown;

//        return isbn.Length switch
//        {
//            ISBN10_LENGTH when IsValidIsbn10(isbn) => SetType(out type, IsbnType.Isbn10),
//            ISBN13_LENGTH when IsValidIsbn13(isbn) => SetType(out type, IsbnType.Isbn13),
//            _ => false
//        };

//        static bool SetType(out IsbnType t, IsbnType value)
//        {
//            t = value;
//            return true;
//        }
//    }

//    private static bool IsValidIsbn10(string isbn)
//    {
//        if (!Isbn10Regex().IsMatch(isbn))
//            return false;

//        int sum = 0;

//        for (int i = 0; i < 9; i++)
//        {
//            sum += (isbn[i] - '0') * (10 - i);
//        }

//        char lastChar = isbn[9];
//        int checkDigit = lastChar is 'X' or 'x' ? 10 : lastChar - '0';

//        return (sum + checkDigit) % 11 == 0;
//    }

//    private static bool IsValidIsbn13(string isbn)
//    {
//        if (!Isbn13Regex().IsMatch(isbn))
//            return false;

//        int sum = 0;

//        for (int i = 0; i < 13; i++)
//        {
//            int digit = isbn[i] - '0';
//            sum += (i & 1) == 0 ? digit : digit * 3; // Bitwise check for even/odd
//        }

//        return sum % 10 == 0;
//    }

//    /// <summary>
//    /// Gets the formatted ISBN with hyphens (cached for performance).
//    /// </summary>
//    public string Formatted => _formattedCache ??= FormatIsbn();

//    private string FormatIsbn()
//    {
//        return Type switch
//        {
//            IsbnType.Isbn13 => $"{Value[..3]}-{Value[3]}-{Value.Substring(4, 4)}-{Value.Substring(8, 4)}-{Value[12]}",
//            IsbnType.Isbn10 => $"{Value[0]}-{Value.Substring(1, 4)}-{Value.Substring(5, 4)}-{Value[9]}",
//            _ => Value
//        };
//    }

//    /// <summary>
//    /// Checks if this ISBN is in ISBN-13 format.
//    /// </summary>
//    public bool IsIsbn13 => Type == IsbnType.Isbn13;

//    /// <summary>
//    /// Checks if this ISBN is in ISBN-10 format.
//    /// </summary>
//    public bool IsIsbn10 => Type == IsbnType.Isbn10;

//    protected override IEnumerable<object?> GetEqualityComponents()
//    {
//        yield return Value;
//    }

//    public override string ToString() => Formatted;

//    public static implicit operator string(ISBN isbn) => isbn.Value;

//    [GeneratedRegex(@"^[0-9]{9}[0-9Xx]$", RegexOptions.Compiled)]
//    private static partial Regex Isbn10Regex();

//    [GeneratedRegex(@"^[0-9]{13}$", RegexOptions.Compiled)]
//    private static partial Regex Isbn13Regex();
//}

///// <summary>
///// Represents the type of ISBN format.
///// </summary>
//public enum IsbnType
//{
//    Unknown = 0,
//    Isbn10 = 10,
//    Isbn13 = 13
//}



