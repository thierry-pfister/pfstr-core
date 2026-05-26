namespace Pfstr.Infrastructure.Data.Entities;

public class PostEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Content { get; set; }
    public string? CoverImage { get; set; }
    public string? CanonicalUrl { get; set; }
    public int? ReadingMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new List<string>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
}
