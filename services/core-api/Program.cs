using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Pfstr.Api.Auth;
using Pfstr.Application.Posts;
using Pfstr.Application.Projects;
using Pfstr.Infrastructure.Data;
using Pfstr.Infrastructure.Migrations;
using Pfstr.Infrastructure.Posts;
using Pfstr.Infrastructure.Projects;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")!;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;

builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(M20260101001_CreateProjectsTable).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<IProjectRepository>(sp =>
    new CachedProjectRepository(sp.GetRequiredService<ProjectRepository>(), sp.GetRequiredService<IDistributedCache>()));
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<IPostRepository>(sp =>
    new CachedPostRepository(sp.GetRequiredService<PostRepository>(), sp.GetRequiredService<IDistributedCache>()));
builder.Services
    .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        var components = document.Components ?? new OpenApiComponents();
        document.Components = components;
        components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-Api-Key",
            Description = "API key for write operations"
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
