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

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")!;

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(M20260101001_CreateProjectsTable).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
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
