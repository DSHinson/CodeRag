using FluentMigrator;

namespace CodeRag.db.Migrations
{
    /// <summary>
    /// Migration to create the "repositories" table in the database.
    /// </summary>
    [Migration(2026_08_02_1)]
    public sealed class CreateRepositoriesTable : Migration
    {
        public override void Down()
        {
            Delete.Table("repositories");
        }

        public override void Up()
        {
            Create.Table("repositories")
              .WithColumn("id").AsInt32().PrimaryKey().Identity()
              .WithColumn("path").AsString().NotNullable().Unique()
              .WithColumn("added_at").AsDateTime().NotNullable();
        }
    }
}
