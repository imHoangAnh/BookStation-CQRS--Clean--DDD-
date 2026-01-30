using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.SharedKernel;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        OccurredOnUtc = DateTime.UtcNow;
    }

    public DateTime OccurredOnUtc { get; }
}

