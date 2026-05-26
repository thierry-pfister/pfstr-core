using FluentMigrator;

namespace Pfstr.Infrastructure.Migrations;

[Migration(20260526001)]
public class M20260526001_AddPostRichFields : Migration
{
    public override void Up()
    {
        Alter.Table("posts")
            .AddColumn("subtitle").AsString(int.MaxValue).Nullable()
            .AddColumn("cover_image").AsString(int.MaxValue).Nullable()
            .AddColumn("canonical_url").AsString(int.MaxValue).Nullable()
            .AddColumn("reading_minutes").AsInt32().Nullable();
    }

    public override void Down()
    {
        Delete.Column("subtitle").FromTable("posts");
        Delete.Column("cover_image").FromTable("posts");
        Delete.Column("canonical_url").FromTable("posts");
        Delete.Column("reading_minutes").FromTable("posts");
    }
}
