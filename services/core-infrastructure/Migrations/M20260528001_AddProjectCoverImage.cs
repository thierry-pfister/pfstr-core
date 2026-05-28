using FluentMigrator;

namespace Pfstr.Infrastructure.Migrations;

[Migration(20260528001)]
public class M20260528001_AddProjectCoverImage : Migration
{
    public override void Up()
    {
        Alter.Table("projects")
            .AddColumn("cover_image_url").AsString(int.MaxValue).Nullable();
    }

    public override void Down()
    {
        Delete.Column("cover_image_url").FromTable("projects");
    }
}
