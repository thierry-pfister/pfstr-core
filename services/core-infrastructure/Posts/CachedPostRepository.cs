using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Pfstr.Application.Posts;
using Pfstr.Domain;
using Pfstr.Domain.Posts;
using Pfstr.Infrastructure.Data.Entities;

namespace Pfstr.Infrastructure.Posts;

public class CachedPostRepository(IPostRepository inner, IDistributedCache cache) : IPostRepository
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    private static string SlugKey(string slug) => $"posts:slug:{slug}";
    private const string AllKey = "posts:all";

    public Task<FSharpOption<Post>> FindById(PostId id) => inner.FindById(id);

    public async Task<FSharpOption<Post>> FindBySlug(Slug.T slug)
    {
        var key = SlugKey(Slug.value(slug));
        var cached = await cache.GetStringAsync(key);
        if (cached != null)
        {
            var entity = JsonSerializer.Deserialize<PostEntity>(cached)!;
            return new FSharpOption<Post>(PostMapper.ToDomain(entity));
        }
        var result = await inner.FindBySlug(slug);
        if (OptionModule.IsSome(result))
            await cache.SetStringAsync(key, JsonSerializer.Serialize(PostMapper.ToEntity(result.Value)), CacheOptions);
        return result;
    }

    public async Task<FSharpList<Post>> FindAll()
    {
        var cached = await cache.GetStringAsync(AllKey);
        if (cached != null)
        {
            var entities = JsonSerializer.Deserialize<List<PostEntity>>(cached)!;
            return ListModule.OfSeq(entities.Select(PostMapper.ToDomain));
        }
        var posts = await inner.FindAll();
        var toCache = posts.Select(PostMapper.ToEntity).ToList();
        await cache.SetStringAsync(AllKey, JsonSerializer.Serialize(toCache), CacheOptions);
        return posts;
    }

    public async Task Save(Post post)
    {
        await inner.Save(post);
        await cache.RemoveAsync(AllKey);
        await cache.RemoveAsync(SlugKey(Slug.value(post.Slug)));
    }
}
