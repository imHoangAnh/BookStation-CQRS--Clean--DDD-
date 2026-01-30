using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.SharedKernel
{
    /// <summary>Maker interface for entities</summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// Generic marker interface for entities with a specific ID type.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntity<TId> : IEntity
        where TId : IEquatable<TId>
    {
        TId Id { get; }
    }
}

/// <summary>
/// IEquatable: Defines a generalized method that a value type or class implements 
/// to create a type-specific method (phuong phap dac thu) 
/// for determining equality of instances (xac dinh su bang nhau). 
/// </summary>
