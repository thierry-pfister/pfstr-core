namespace Pfstr.Infrastructure.Data.Entities;

public class ProjectEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Summary { get; set; } = "";
    public string? Content { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Status { get; set; } = "Draft";
    public List<string> TechStack { get; set; } = [];
    public List<ProjectLinkDto> Links { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public int DisplayOrder { get; set; }
}
