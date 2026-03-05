using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.SharedKernel;


public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : IEquatable<TId>
{
    private readonly List<IDomainEvent> _domainEvents;

    /// <remarks>
    /// Required by EF Core.
    /// </remarks>
    protected AggregateRoot()
    {
        _domainEvents = [];
    }

    protected AggregateRoot(TId id) : base(id)
    {
        _domainEvents = []; 
    }


    /// <summary>
    /// Gets the domain events snapshot.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => [.. _domainEvents];

    /// <summary>
    /// Clears all the domain events
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Adds the specified
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected void AddDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent => _domainEvents.Add(domainEvent);
}
