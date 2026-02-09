using BookStation.Core.SharedKernel;

namespace BookStation.Domain.ValueObjects;

/// <summary>
/// Value object for ISBN-13 (International Standard Book Number).
/// Since January 1, 2007, all ISBNs are 13 digits long.
/// </summary>
public sealed class ISBN : ValueObject
{
    private const int ISBN_LENGTH = 13;

    /// <summary>
    /// Gets the normalized ISBN-13 value (without hyphens or spaces).
    /// </summary>
    public string Value { get; }

    private string? _formattedCache;

    private ISBN(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new ISBN-13 value object.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when ISBN is invalid.</exception>
    public static ISBN Create(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));

        var cleaned = NormalizeIsbn(isbn);

        if (IsValidIsbn13(cleaned))
            return new ISBN(cleaned);

        throw new ArgumentException($"Invalid ISBN-13 format: {isbn}", nameof(isbn));
    }

    /// <summary>
    /// Tries to create a new ISBN-13 value object.
    /// </summary>
    public static bool TryCreate(string? isbn, out ISBN? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(isbn))
            return false;

        var cleaned = NormalizeIsbn(isbn);

        if (IsValidIsbn13(cleaned))
        {
            result = new ISBN(cleaned);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Normalizes ISBN by removing hyphens and spaces.
    /// </summary>
    private static string NormalizeIsbn(string isbn)
    {
        Span<char> buffer = stackalloc char[isbn.Length];
        int index = 0;

        foreach (char c in isbn)
        {
            if (c != '-' && c != ' ')
            {
                buffer[index++] = c;
            }
        }

        return new string(buffer[..index]);
    }

    /// <summary>
    /// Validates ISBN-13 format and checksum.
    /// </summary>
    private static bool IsValidIsbn13(ReadOnlySpan<char> isbn)
    {
        // Must be exactly 13 characters
        if (isbn.Length != ISBN_LENGTH)
            return false;

        // Calculate checksum and validate format in one pass
        int sum = 0;
        for (int i = 0; i < ISBN_LENGTH; i++)
        {
            char c = isbn[i];

            // Must be a digit
            if (!char.IsDigit(c))
                return false;

            // Calculate checksum (alternating weights: 1, 3, 1, 3, ...)
            int digit = c - '0';
            sum += (i & 1) == 0 ? digit : digit * 3;
        }

        // Checksum must be divisible by 10
        return sum % 10 == 0;
    }

    /// <summary>
    /// Gets the formatted ISBN-13 with hyphens (cached for performance).
    /// Format: 978-0-306-40615-7
    /// </summary>
    public string Formatted => _formattedCache ??= FormatIsbn13();

    private string FormatIsbn13()
    {
        // Format: 978-0-306-40615-7 (13 digits + 4 hyphens = 17 chars)
        Span<char> buffer = stackalloc char[17];

        // Prefix (3 digits): 978 or 979
        Value.AsSpan(0, 3).CopyTo(buffer);
        buffer[3] = '-';

        // Registration group (1 digit - simplified)
        buffer[4] = Value[3];
        buffer[5] = '-';

        // Registrant (4 digits - simplified)
        Value.AsSpan(4, 4).CopyTo(buffer[6..]);
        buffer[10] = '-';

        // Publication (4 digits - simplified)
        Value.AsSpan(8, 4).CopyTo(buffer[11..]);
        buffer[15] = '-';

        // Check digit (1 digit)
        buffer[16] = Value[12];

        return new string(buffer);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Formatted;

    public static implicit operator string(ISBN isbn) => isbn.Value;
}


/// <summary>
/// ISBN-13 dựa trên chuẩn EAN-13 (European Article Number)
/// → EAN-13 dùng mod 10
/// → Tương thích với barcode quốc tế
/// → Dễ tích hợp với hệ thống bán lẻ toàn cầu
/// </summary>