using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FSharp.Core;
using Pfstr.Api.Helpers;
using Pfstr.Api.Models;
using Pfstr.Application.Posts;
using Pfstr.Domain;
using Pfstr.Domain.Posts;

namespace Pfstr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(IPostRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostResponse>>> List([FromQuery] string? status = null)
    {
        var posts = await ListPosts.handle(repo, new ListPosts.Query(ParseStatus(status))).ToTask();
        return Ok(posts.Select(ToResponse));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<PostResponse>> Get(string slug)
    {
        var result = await GetPost.handle(repo, new GetPost.Query(slug)).ToTask();
        return result.IsOk ? Ok(ToResponse(result.ResultValue)) : MapError(result.ErrorValue);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreatePostResponse>> Create([FromBody] CreatePostRequest request)
    {
        var cmd = new CreatePost.Command(request.Title, request.Slug, request.Summary);
        var result = await CreatePost.handle(repo, DateTimeOffset.UtcNow, cmd).ToTask();
        if (result.IsOk)
            return CreatedAtAction(nameof(Get), new { slug = request.Slug }, new CreatePostResponse(result.ResultValue.Item));
        return MapError(result.ErrorValue);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdatePostRequest request)
    {
        var subtitle    = request.Subtitle    is null ? FSharpOption<string>.None : new FSharpOption<string>(request.Subtitle);
        var content     = request.Content     is null ? FSharpOption<string>.None : new FSharpOption<string>(request.Content);
        var coverImage  = request.CoverImage  is null ? FSharpOption<string>.None : new FSharpOption<string>(request.CoverImage);
        var canonicalUrl = request.CanonicalUrl is null ? FSharpOption<string>.None : new FSharpOption<string>(request.CanonicalUrl);
        var tags = Microsoft.FSharp.Collections.ListModule.OfSeq(request.Tags ?? new List<string>());
        var cmd = new UpdatePost.Command(id, request.Title, request.Summary, subtitle, content, coverImage, canonicalUrl, tags);
        var result = await UpdatePost.handle(repo, cmd).ToTask();
        return result.IsOk ? NoContent() : MapError(result.ErrorValue);
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize]
    public async Task<ActionResult> Publish(Guid id)
    {
        var result = await PublishPost.handle(repo, DateTimeOffset.UtcNow, new PublishPost.Command(id)).ToTask();
        return result.IsOk ? NoContent() : MapError(result.ErrorValue);
    }

    [HttpPost("{id:guid}/archive")]
    [Authorize]
    public async Task<ActionResult> Archive(Guid id)
    {
        var result = await ArchivePost.handle(repo, DateTimeOffset.UtcNow, new ArchivePost.Command(id)).ToTask();
        return result.IsOk ? NoContent() : MapError(result.ErrorValue);
    }

    private static PostResponse ToResponse(Post p) => new(
        p.Id.Item,
        p.Title,
        Slug.value(p.Slug),
        p.Summary,
        p.Subtitle     is null ? null : p.Subtitle.Value,
        p.Content      is null ? null : p.Content.Value,
        p.CoverImage   is null ? null : p.CoverImage.Value,
        p.CanonicalUrl is null ? null : p.CanonicalUrl.Value,
        p.ReadingMinutes is null ? null : p.ReadingMinutes.Value,
        p.Status.ToString(),
        p.Tags.ToList(),
        p.CreatedAt,
        p.PublishedAt  is null ? null : p.PublishedAt.Value
    );

    private ActionResult MapError(PostApplicationError error) => error switch
    {
        PostApplicationError.ValidationError ve => BadRequest(new { error = ve.Item }),
        PostApplicationError.NotFound nf        => NotFound(new { error = nf.Item }),
        PostApplicationError.Conflict c         => Conflict(new { error = c.Item }),
        PostApplicationError.DomainError de     => UnprocessableEntity(new { error = de.Item }),
        _                                       => StatusCode(500)
    };

    private static FSharpOption<PostStatus> ParseStatus(string? s) => s switch
    {
        "Draft"     => new FSharpOption<PostStatus>(PostStatus.Draft),
        "Published" => new FSharpOption<PostStatus>(PostStatus.Published),
        "Archived"  => new FSharpOption<PostStatus>(PostStatus.Archived),
        _           => FSharpOption<PostStatus>.None
    };
}
