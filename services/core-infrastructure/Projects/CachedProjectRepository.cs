using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Pfstr.Application.Projects;
using Pfstr.Domain;
using Pfstr.Domain.Projects;
using Pfstr.Infrastructure.Data.Entities;

namespace Pfstr.Infrastructure.Projects;

public class CachedProjectRepository(IProjectRepository inner, IDistributedCache cache) : IProjectRepository
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    private static string SlugKey(string slug) => $"projects:slug:{slug}";
    private const string AllKey = "projects:all";

    public Task<FSharpOption<Project>> FindById(ProjectId id) => inner.FindById(id);

    public async Task<FSharpOption<Project>> FindBySlug(Slug.T slug)
    {
        var key = SlugKey(Slug.value(slug));
        var cached = await cache.GetStringAsync(key);
        if (cached != null)
        {
            var entity = JsonSerializer.Deserialize<ProjectEntity>(cached)!;
            return new FSharpOption<Project>(ProjectMapper.ToDomain(entity));
        }
        var result = await inner.FindBySlug(slug);
        if (OptionModule.IsSome(result))
            await cache.SetStringAsync(key, JsonSerializer.Serialize(ProjectMapper.ToEntity(result.Value)), CacheOptions);
        return result;
    }

    public async Task<FSharpList<Project>> FindAll()
    {
        var cached = await cache.GetStringAsync(AllKey);
        if (cached != null)
        {
            var entities = JsonSerializer.Deserialize<List<ProjectEntity>>(cached)!;
            return ListModule.OfSeq(entities.Select(ProjectMapper.ToDomain));
        }
        var projects = await inner.FindAll();
        var toCache = projects.Select(ProjectMapper.ToEntity).ToList();
        await cache.SetStringAsync(AllKey, JsonSerializer.Serialize(toCache), CacheOptions);
        return projects;
    }

    public async Task Save(Project project)
    {
        await inner.Save(project);
        await cache.RemoveAsync(AllKey);
        await cache.RemoveAsync(SlugKey(Slug.value(project.Slug)));
    }
}
