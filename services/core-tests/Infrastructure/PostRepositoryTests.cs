using Microsoft.FSharp.Core;
using Pfstr.Domain;
using Pfstr.Domain.Posts;
using Pfstr.Infrastructure.Posts;

namespace Pfstr.Tests.Infrastructure;

public class PostRepositoryTests(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

    private static Post NewPost(string slugStr = "test-post")
    {
        var slug = Slug.create(slugStr) switch { var r when r.IsOk => r.ResultValue, _ => throw new Exception() };
        return PostModule.create(PostId.NewPostId(Guid.NewGuid()), "Test Post", slug, "Summary", Now);
    }

    private PostRepository Repo() => new(db.CreateDbContext());

    [Fact]
    public async Task Save_ThenFindById_ReturnsPost()
    {
        var post = NewPost("find-by-id-post");
        var repo = Repo();

        await repo.Save(post);
        var found = await repo.FindById(post.Id);

        Assert.True(OptionModule.IsSome(found));
        Assert.Equal(post.Id, found.Value.Id);
    }

    [Fact]
    public async Task FindBySlug_ReturnsMatchingPost()
    {
        var post = NewPost("find-by-slug-post");
        var slug = Slug.create("find-by-slug-post") switch { var r when r.IsOk => r.ResultValue, _ => throw new Exception() };
        var repo = Repo();

        await repo.Save(post);
        var found = await repo.FindBySlug(slug);

        Assert.True(OptionModule.IsSome(found));
        Assert.Equal(Slug.value(post.Slug), Slug.value(found.Value.Slug));
    }

    [Fact]
    public async Task FindAll_IncludesSavedPosts()
    {
        var repo = Repo();
        await repo.Save(NewPost("find-all-post-1"));
        await repo.Save(NewPost("find-all-post-2"));

        var all = await repo.FindAll();

        Assert.True(all.Length >= 2);
    }

    [Fact]
    public async Task Save_UpdatesExistingPost()
    {
        var post = NewPost("save-update-post");
        var repo = Repo();

        await repo.Save(post);
        var published = PostModule.publish(Now, post) switch { var r when r.IsOk => r.ResultValue, _ => throw new Exception() };
        await repo.Save(published);

        var found = await repo.FindById(post.Id);
        Assert.Equal(PostStatus.Published, found.Value.Status);
    }

    [Fact]
    public async Task FindById_ReturnsNone_WhenNotFound()
    {
        var found = await Repo().FindById(PostId.NewPostId(Guid.NewGuid()));
        Assert.True(OptionModule.IsNone(found));
    }
}
