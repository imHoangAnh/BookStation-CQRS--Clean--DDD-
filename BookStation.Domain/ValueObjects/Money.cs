using BookStation.Core.SharedKernel;

namespace BookStation.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }
    public const string DefaultCurrency = "VND";

    private Money() { Amount = 0; Currency = DefaultCurrency; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = DefaultCurrency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));
        return new Money(amount, currency.ToUpperInvariant());
    }

    public static Money Zero(string currency = DefaultCurrency) => new(0, currency);

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot perform operation on different currencies: {Currency} and {other.Currency}");
    }

    public Money Add(Money other) { EnsureSameCurrency(other); return new Money(Amount + other.Amount, Currency); }
    public Money Subtract(Money other) { EnsureSameCurrency(other); var r = Amount - other.Amount; return new Money(r >= 0 ? r : 0, Currency); }
    public Money Multiply(decimal factor) { if (factor < 0) throw new ArgumentException("Factor cannot be negative.", nameof(factor)); return new Money(Amount * factor, Currency); }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N0} {Currency}";

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money left, decimal right) => left.Multiply(right);
    public static bool operator >(Money left, Money right) => left.Amount > right.Amount;
    public static bool operator <(Money left, Money right) => left.Amount < right.Amount;
    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;
    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;
}
