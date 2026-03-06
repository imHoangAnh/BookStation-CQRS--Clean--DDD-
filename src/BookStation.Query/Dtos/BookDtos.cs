namespace BookStation.Query.Dtos;

public sealed record BookListDto(
    long Id,
    string Title,
    string? Language,
    int? PublishYear);

public sealed record BookDetailDto(
    long Id,
    string Title,
    string? Description,
    string? Language,
    int? PublishYear,
    Guid? PublisherId,
    string? PublisherName,
    string? CoverImageUrl,
    int? PageCount);
