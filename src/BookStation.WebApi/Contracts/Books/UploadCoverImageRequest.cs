using Microsoft.AspNetCore.Http;

namespace BookStation.WebApi.Contracts.Books;

public sealed class UploadCoverImageRequest
{
    public required IFormFile File { get; init; }
}

