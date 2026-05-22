using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Pfstr.Domain;
using Pfstr.Domain.Posts;
using Pfstr.Infrastructure.Data.Entities;

namespace Pfstr.Infrastructure.Posts;

internal static class PostMapper
{
    public static Post ToDomain(PostEntity entity)
    {
        var slugResult = Slug.create(entity.Slug);
        if (!slugResult.IsOk)
            throw new InvalidOperationException($"Database contains invalid slug: '{entity.Slug}'");

        return new Post(
            PostId.NewPostId(entity.Id),
            entity.Title,
            slugResult.ResultValue,
            entity.Summary,
            entity.Content is null ? FSharpOption<string>.None : new FSharpOption<string>(entity.Content),
            ParseStatus(entity.Status),
            ListModule.OfSeq(entity.Tags),
            entity.CreatedAt,
            entity.PublishedAt is null ? FSharpOption<DateTimeOffset>.None : new FSharpOption<DateTimeOffset>(entity.PublishedAt.Value)
        );
    }

    public static PostEntity ToEntity(Post post) => new()
    {
        Id = post.Id.Item,
        Title = post.Title,
        Slug = Slug.value(post.Slug),
        Summary = post.Summary,
        Content = post.Content is null ? null : post.Content.Value,
        Status = FormatStatus(post.Status),
        Tags = post.Tags.ToList(),
        CreatedAt = post.CreatedAt,
        PublishedAt = post.PublishedAt is null ? null : post.PublishedAt.Value
    };

    private static PostStatus ParseStatus(string status) => status switch
    {
        "Draft"     => PostStatus.Draft,
        "Published" => PostStatus.Published,
        "Archived"  => PostStatus.Archived,
        _           => throw new InvalidOperationException($"Unknown post status: '{status}'")
    };

    private static string FormatStatus(PostStatus status)
    {
        if (status.IsDraft)     return "Draft";
        if (status.IsPublished) return "Published";
        if (status.IsArchived)  return "Archived";
        throw new InvalidOperationException("Unknown post status");
    }
}
