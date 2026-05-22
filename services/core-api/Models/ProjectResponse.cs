namespace Pfstr.Api.Models;

public record ProjectResponse(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string? Content,
    string Status,
    List<string> TechStack,
    List<ProjectLinkResponse> Links,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt,
    int DisplayOrder
);

public record ProjectLinkResponse(string Label, string Url);

public record CreateProjectRequest(string Title, string Slug, string Summary);

public record CreateProjectResponse(Guid Id);

public record UpdateProjectRequest(
    string Title,
    string Summary,
    string? Content,
    List<string> TechStack,
    List<ProjectLinkRequest> Links,
    int DisplayOrder
);

public record ProjectLinkRequest(string Label, string Url);
