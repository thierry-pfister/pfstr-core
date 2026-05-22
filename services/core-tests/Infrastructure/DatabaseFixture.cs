using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pfstr.Infrastructure.Data;
using Pfstr.Infrastructure.Migrations;
using Testcontainers.PostgreSql;

namespace Pfstr.Tests.Infrastructure;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
        return new AppDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        RunMigrations();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    private void RunMigrations()
    {
        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(_container.GetConnectionString())
                .ScanIn(typeof(M20260101001_CreateProjectsTable).Assembly).For.Migrations())
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }
}
