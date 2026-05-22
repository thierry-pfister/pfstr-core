using FluentMigrator;

namespace Pfstr.Infrastructure.Migrations;

[Migration(20260101002)]
public class M20260101002_CreatePostsTable : Migration
{
    public override void Up()
    {
        Create.Table("posts")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("title").AsString(int.MaxValue).NotNullable()
            .WithColumn("slug").AsString(100).NotNullable()
            .WithColumn("summary").AsString(int.MaxValue).NotNullable()
            .WithColumn("content").AsString(int.MaxValue).Nullable()
            .WithColumn("status").AsString(20).NotNullable().WithDefaultValue("Draft")
            .WithColumn("tags").AsCustom("jsonb").NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable()
            .WithColumn("published_at").AsDateTimeOffset().Nullable();

        Create.Index("idx_posts_slug").OnTable("posts").OnColumn("slug").Unique();
        Create.Index("idx_posts_status").OnTable("posts").OnColumn("status");
    }

    public override void Down()
    {
        Delete.Table("posts");
    }
}
