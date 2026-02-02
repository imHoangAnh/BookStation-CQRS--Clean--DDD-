using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.SharedKernel;
/// <summary>
/// Base class for all entities.
/// </summary>
public abstract class Entity<TId> : IEntity<TId>
where TId : IEquatable<TId>
{
    /// <summary>
    /// Gets/sets the entity identifier.
    /// </summary>
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    protected Entity()
    {
    }
    protected Entity(TId id)
    {
        Id = id;
    }
}


/// <summary> 
/// public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow; 
/// using for big systems with multiple time zones
/// </summary>