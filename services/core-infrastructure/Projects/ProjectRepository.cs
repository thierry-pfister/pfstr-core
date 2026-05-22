using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Pfstr.Application.Projects;
using Pfstr.Domain.Projects;
using Pfstr.Infrastructure.Data;

namespace Pfstr.Infrastructure.Projects;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    public async Task<FSharpOption<Project>> FindById(ProjectId id)
    {
        var entity = await context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id.Item);

        return entity is null
            ? FSharpOption<Project>.None
            : new FSharpOption<Project>(ProjectMapper.ToDomain(entity));
    }

    public async Task<FSharpOption<Project>> FindBySlug(Slug.T slug)
    {
        var entity = await context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == Slug.value(slug));

        return entity is null
            ? FSharpOption<Project>.None
            : new FSharpOption<Project>(ProjectMapper.ToDomain(entity));
    }

    public async Task<FSharpList<Project>> FindAll()
    {
        var entities = await context.Projects
            .AsNoTracking()
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync();

        return ListModule.OfSeq(entities.Select(ProjectMapper.ToDomain));
    }

    public async Task Save(Project project)
    {
        var entity = ProjectMapper.ToEntity(project);
        var exists = await context.Projects.AnyAsync(p => p.Id == entity.Id);

        if (!exists)
            context.Projects.Add(entity);
        else
            context.Projects.Update(entity);

        await context.SaveChangesAsync();
    }
}
