using BookStation.Core.SharedKernel;

namespace BookStation.Domain.ValueObjects;

/// <summary>
/// Value object representing a physical address.
/// </summary>
public sealed class Address : ValueObject
{
    public string Street { get; }
    public string Ward { get; }
    public string City { get; }
    public string Country { get; }
    public string? PostalCode { get; }

    private Address(string street, string ward, string city, string country, string? postalCode)
    {
        Street = street;
        Ward = ward;
        City = city;
        Country = country;
        PostalCode = postalCode;
    }

    /// <summary>
    /// Creates a new Address value object.
    /// </summary>
    public static Address Create(
        string street,
        string city,
        string country,
        string ward,
        string? postalCode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(ward);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);

        return new Address(
                street.Trim(),
                ward.Trim(),
                city.Trim(),
                country.Trim(),
                postalCode?.Trim()
            );
    }

    /// <summary>
    /// Gets the full formatted address.
    /// </summary>
    public string FullAddress
    {
        get
        {
            var parts = new List<string> { Street };

            if (!string.IsNullOrWhiteSpace(Ward))
                parts.Add(Ward);
            if (!string.IsNullOrWhiteSpace(City))
                parts.Add(City);

            if (!string.IsNullOrWhiteSpace(PostalCode))
                parts.Add(PostalCode);

            parts.Add(Country);

            return string.Join(", ", parts);
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return Ward;
        yield return City;
        yield return Country;
        yield return PostalCode;
    }

    public override string ToString() => FullAddress;
}

