using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BookStation.Core.SharedKernel;

/// <summary>
/// Base class for Value Objects.
/// Value objects is an object with three characteristics: imutable, supports structure equality, and no identity.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Derived classes must specify which values are used for comparison.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Compares two value objects by their equality components.
    /// </summary>
    public bool Equals(ValueObject? other)
    {
        if (other is null || GetType() != other.GetType())
        {
            return false;
        }

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    /// <summary>
    /// Overrides object.Equals to support value-based equality.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not ValueObject other)
        {
            return false;
        }
        return Equals(other);
    }

    /// <summary>
    /// Generates a hash code based on the equality components.
    /// </summary>
    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var value in GetEqualityComponents())
        {
            hash.Add(value);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// Equality operator compares value objects by value.
    /// </summary>
    public static bool operator == (ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator (logical opposite of ==).   
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

}