using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using Pfstr.Application.Posts;
using Pfstr.Domain;
using Pfstr.Domain.Posts;
using Pfstr.Infrastructure.Data;

namespace Pfstr.Infrastructure.Posts;

public class PostRepository(AppDbContext context) : IPostRepository
{
    public async Task<FSharpOption<Post>> FindById(PostId id)
    {
        var entity = await context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id.Item);
        return entity is null ? FSharpOption<Post>.None : new FSharpOption<Post>(PostMapper.ToDomain(entity));
    }

    public async Task<FSharpOption<Post>> FindBySlug(Slug.T slug)
    {
        var slugValue = Slug.value(slug);
        var entity = await context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slugValue);
        return entity is null ? FSharpOption<Post>.None : new FSharpOption<Post>(PostMapper.ToDomain(entity));
    }

    public async Task<Microsoft.FSharp.Collections.FSharpList<Post>> FindAll()
    {
        var entities = await context.Posts.AsNoTracking().ToListAsync();
        return Microsoft.FSharp.Collections.ListModule.OfSeq(entities.Select(PostMapper.ToDomain));
    }

    public async Task Save(Post post)
    {
        var entity = PostMapper.ToEntity(post);
        var exists = await context.Posts.AnyAsync(p => p.Id == entity.Id);
        if (exists)
            context.Posts.Update(entity);
        else
            context.Posts.Add(entity);
        await context.SaveChangesAsync();
    }
}
