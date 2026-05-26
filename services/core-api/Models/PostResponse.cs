namespace Pfstr.Api.Models;

public record PostResponse(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string? Subtitle,
    string? Content,
    string? CoverImage,
    string? CanonicalUrl,
    int? ReadingMinutes,
    string Status,
    List<string> Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt
);

public record CreatePostRequest(string Title, string Slug, string Summary);

public record CreatePostResponse(Guid Id);

public record UpdatePostRequest(
    string Title,
    string Summary,
    string? Subtitle,
    string? Content,
    string? CoverImage,
    string? CanonicalUrl,
    List<string> Tags
);
