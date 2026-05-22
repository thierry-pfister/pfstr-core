namespace Pfstr.Api.Models;

public record PostResponse(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string? Content,
    string Status,
    List<string> Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt
);

public record CreatePostRequest(string Title, string Slug, string Summary);

public record CreatePostResponse(Guid Id);
