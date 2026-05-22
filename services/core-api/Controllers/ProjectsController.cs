using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FSharp.Core;
using Pfstr.Api.Helpers;
using Pfstr.Api.Models;
using Pfstr.Application.Projects;
using Pfstr.Domain;
using Pfstr.Domain.Projects;

namespace Pfstr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> List([FromQuery] string? status = null)
    {
        var projects = await ListProjects.handle(repo, new ListProjects.Query(ParseStatus(status))).ToTask();
        return Ok(projects.Select(ToResponse));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ProjectResponse>> Get(string slug)
    {
        var result = await GetProject.handle(repo, new GetProject.Query(slug)).ToTask();
        return result.IsOk ? Ok(ToResponse(result.ResultValue)) : MapError(result.ErrorValue);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreateProjectResponse>> Create([FromBody] CreateProjectRequest request)
    {
        var cmd = new CreateProject.Command(request.Title, request.Slug, request.Summary);
        var result = await CreateProject.handle(repo, DateTimeOffset.UtcNow, cmd).ToTask();
        if (result.IsOk)
            return CreatedAtAction(nameof(Get), new { slug = request.Slug }, new CreateProjectResponse(result.ResultValue.Item));
        return MapError(result.ErrorValue);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
    {
        var links = request.Links.Select(l => new ProjectLink(l.Label, l.Url)).ToList();
        var content = request.Content is null ? FSharpOption<string>.None : new FSharpOption<string>(request.Content);
        var techStack = request.TechStack ?? new List<string>();
        var cmd = new UpdateProject.Command(id, request.Title, request.Summary, content,
            Microsoft.FSharp.Collections.ListModule.OfSeq(techStack),
            Microsoft.FSharp.Collections.ListModule.OfSeq(links),
            request.DisplayOrder);
        var result = await UpdateProject.handle(repo, cmd).ToTask();
        return result.IsOk ? NoContent() : MapError(result.ErrorValue);
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize]
    public async Task<ActionResult> Publish(Guid id)
    {
        var result = await PublishProject.handle(repo, DateTimeOffset.UtcNow, new PublishProject.Command(id)).ToTask();
        return result.IsOk ? NoContent() : MapError(result.ErrorValue);
    }

    [HttpPost("{id:guid}/archive")]
    [Authorize]
    public async Task<ActionResult> Archive(Guid id)
    {
        var result = await ArchiveProject.handle(repo, DateTimeOffset.UtcNow, new ArchiveProject.Command(id)).ToTask();
        return result.IsOk ? NoContent() : MapError(result.ErrorValue);
    }

    private static ProjectResponse ToResponse(Project p) => new(
        p.Id.Item,
        p.Title,
        Slug.value(p.Slug),
        p.Summary,
        p.Content is null ? null : p.Content.Value,
        p.Status.ToString(),
        p.TechStack.ToList(),
        p.Links.Select(l => new ProjectLinkResponse(l.Label, l.Url)).ToList(),
        p.CreatedAt,
        p.PublishedAt is null ? null : p.PublishedAt.Value,
        p.DisplayOrder
    );

    private ActionResult MapError(ProjectApplicationError error) => error switch
    {
        ProjectApplicationError.ValidationError ve => BadRequest(new { error = ve.Item }),
        ProjectApplicationError.NotFound nf        => NotFound(new { error = nf.Item }),
        ProjectApplicationError.Conflict c         => Conflict(new { error = c.Item }),
        ProjectApplicationError.DomainError de     => UnprocessableEntity(new { error = de.Item }),
        _                                          => StatusCode(500)
    };

    private static FSharpOption<ProjectStatus> ParseStatus(string? s) => s switch
    {
        "Draft"     => new FSharpOption<ProjectStatus>(ProjectStatus.Draft),
        "Published" => new FSharpOption<ProjectStatus>(ProjectStatus.Published),
        "Archived"  => new FSharpOption<ProjectStatus>(ProjectStatus.Archived),
        _           => FSharpOption<ProjectStatus>.None
    };
}
