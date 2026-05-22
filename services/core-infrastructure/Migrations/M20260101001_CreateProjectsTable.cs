using FluentMigrator;

namespace Pfstr.Infrastructure.Migrations;

[Migration(20260101001)]
public class M20260101001_CreateProjectsTable : Migration
{
    public override void Up()
    {
        Create.Table("projects")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("title").AsString(int.MaxValue).NotNullable()
            .WithColumn("slug").AsString(100).NotNullable()
            .WithColumn("summary").AsString(int.MaxValue).NotNullable()
            .WithColumn("content").AsString(int.MaxValue).Nullable()
            .WithColumn("status").AsString(20).NotNullable().WithDefaultValue("Draft")
            .WithColumn("tech_stack").AsCustom("jsonb").NotNullable()
            .WithColumn("links").AsCustom("jsonb").NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable()
            .WithColumn("published_at").AsDateTimeOffset().Nullable()
            .WithColumn("display_order").AsInt32().NotNullable().WithDefaultValue(0);

        Create.Index("idx_projects_slug").OnTable("projects").OnColumn("slug").Unique();
        Create.Index("idx_projects_status").OnTable("projects").OnColumn("status");
    }

    public override void Down()
    {
        Delete.Table("projects");
    }
}
