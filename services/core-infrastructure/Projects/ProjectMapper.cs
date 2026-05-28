using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Pfstr.Domain;
using Pfstr.Domain.Projects;
using Pfstr.Infrastructure.Data.Entities;

namespace Pfstr.Infrastructure.Projects;

internal static class ProjectMapper
{
    public static Project ToDomain(ProjectEntity entity)
    {
        var slugResult = Slug.create(entity.Slug);
        if (!slugResult.IsOk)
            throw new InvalidOperationException($"Database contains invalid slug: '{entity.Slug}'");

        return new Project(
            ProjectId.NewProjectId(entity.Id),
            entity.Title,
            slugResult.ResultValue,
            entity.Summary,
            entity.Content is null
                ? FSharpOption<string>.None
                : new FSharpOption<string>(entity.Content),
            ParseStatus(entity.Status),
            ListModule.OfSeq(entity.TechStack),
            ListModule.OfSeq(entity.Links.Select(l => new ProjectLink(l.Label, l.Url))),
            entity.CoverImageUrl is null
                ? FSharpOption<string>.None
                : new FSharpOption<string>(entity.CoverImageUrl),
            entity.CreatedAt,
            entity.PublishedAt is null
                ? FSharpOption<DateTimeOffset>.None
                : new FSharpOption<DateTimeOffset>(entity.PublishedAt.Value),
            entity.DisplayOrder
        );
    }

    public static ProjectEntity ToEntity(Project project) => new()
    {
        Id = project.Id.Item,
        Title = project.Title,
        Slug = Slug.value(project.Slug),
        Summary = project.Summary,
        Content = project.Content is null ? null : project.Content.Value,
        CoverImageUrl = project.CoverImageUrl is null ? null : project.CoverImageUrl.Value,
        Status = FormatStatus(project.Status),
        TechStack = project.TechStack.ToList(),
        Links = project.Links.Select(l => new ProjectLinkDto(l.Label, l.Url)).ToList(),
        CreatedAt = project.CreatedAt,
        PublishedAt = project.PublishedAt is null ? null : project.PublishedAt.Value,
        DisplayOrder = project.DisplayOrder
    };

    private static ProjectStatus ParseStatus(string status) => status switch
    {
        "Draft"     => ProjectStatus.Draft,
        "Published" => ProjectStatus.Published,
        "Archived"  => ProjectStatus.Archived,
        _           => throw new InvalidOperationException($"Unknown project status: '{status}'")
    };

    private static string FormatStatus(ProjectStatus status)
    {
        if (status.IsDraft)     return "Draft";
        if (status.IsPublished) return "Published";
        if (status.IsArchived)  return "Archived";
        throw new InvalidOperationException("Unknown project status");
    }
}
