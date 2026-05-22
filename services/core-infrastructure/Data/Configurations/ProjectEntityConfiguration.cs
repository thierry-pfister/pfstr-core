using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pfstr.Infrastructure.Data.Entities;

namespace Pfstr.Infrastructure.Data.Configurations;

public class ProjectEntityConfiguration : IEntityTypeConfiguration<ProjectEntity>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public void Configure(EntityTypeBuilder<ProjectEntity> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Title).HasColumnName("title").IsRequired();
        builder.Property(p => p.Slug).HasColumnName("slug").IsRequired().HasMaxLength(100);
        builder.Property(p => p.Summary).HasColumnName("summary").IsRequired();
        builder.Property(p => p.Content).HasColumnName("content");
        builder.Property(p => p.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.PublishedAt).HasColumnName("published_at");
        builder.Property(p => p.DisplayOrder).HasColumnName("display_order");

        builder.Property(p => p.TechStack)
            .HasColumnName("tech_stack")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>());

        builder.Property(p => p.Links)
            .HasColumnName("links")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<List<ProjectLinkDto>>(v, JsonOptions) ?? new List<ProjectLinkDto>());

        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.Status);
    }
}
