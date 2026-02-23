using BookStation.Core.SharedKernel;

namespace BookStation.Domain.Entities.BookAggregate;

public sealed class BookCreatedEvent(Book book) : DomainEvent { public Book Book { get; } = book; }
public sealed class BookUpdatedEvent(long bookId) : DomainEvent { public long BookId { get; } = bookId; }
public sealed class BookPublishedEvent(long bookId) : DomainEvent { public long BookId { get; } = bookId; }
