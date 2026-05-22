using Microsoft.FSharp.Core;
using Pfstr.Domain.Projects;
using Pfstr.Infrastructure.Projects;

namespace Pfstr.Tests.Infrastructure;

public class ProjectRepositoryTests(DatabaseFixture db) : IClassFixture<DatabaseFixture>
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

    private static Project NewProject(string slugStr = "test-project")
    {
        var slug = Slug.create(slugStr) switch { var r when r.IsOk => r.ResultValue, _ => throw new Exception() };
        return ProjectModule.create(ProjectId.NewProjectId(Guid.NewGuid()), "Test Project", slug, "Summary", Now);
    }

    private ProjectRepository Repo() => new(db.CreateDbContext());

    [Fact]
    public async Task Save_ThenFindById_ReturnsProject()
    {
        var project = NewProject("find-by-id-test");
        var repo = Repo();

        await repo.Save(project);
        var found = await repo.FindById(project.Id);

        Assert.True(OptionModule.IsSome(found));
        Assert.Equal(project.Id, found.Value.Id);
    }

    [Fact]
    public async Task FindBySlug_ReturnsMatchingProject()
    {
        var project = NewProject("find-by-slug-test");
        var slug = Slug.create("find-by-slug-test") switch { var r when r.IsOk => r.ResultValue, _ => throw new Exception() };
        var repo = Repo();

        await repo.Save(project);
        var found = await repo.FindBySlug(slug);

        Assert.True(OptionModule.IsSome(found));
        Assert.Equal(Slug.value(project.Slug), Slug.value(found.Value.Slug));
    }

    [Fact]
    public async Task FindAll_IncludesSavedProjects()
    {
        var repo = Repo();
        await repo.Save(NewProject("find-all-test-1"));
        await repo.Save(NewProject("find-all-test-2"));

        var all = await repo.FindAll();

        Assert.True(all.Length >= 2);
    }

    [Fact]
    public async Task Save_UpdatesExistingProject()
    {
        var project = NewProject("save-update-test");
        var repo = Repo();

        await repo.Save(project);
        var published = ProjectModule.publish(Now, project) switch { var r when r.IsOk => r.ResultValue, _ => throw new Exception() };
        await repo.Save(published);

        var found = await repo.FindById(project.Id);
        Assert.Equal(ProjectStatus.Published, found.Value.Status);
    }

    [Fact]
    public async Task FindById_ReturnsNone_WhenNotFound()
    {
        var found = await Repo().FindById(ProjectId.NewProjectId(Guid.NewGuid()));
        Assert.True(OptionModule.IsNone(found));
    }
}
