using System.Data;
using FluentMigrator;

namespace CodeRag.db.Migrations
{
    [Migration(202608022)]
    public sealed class CreateIndexTables : Migration
    {
        public override void Down()
        {
            Delete.Table("embeddings");
            Delete.Table("chunks");
            Delete.Table("files");
        }

        public override void Up()
        {
            Create.Table("files")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("repository_id").AsInt32().NotNullable()
                    .ForeignKey("fk_files_repositories", "repositories", "id").OnDelete(Rule.Cascade)
                .WithColumn("relative_path").AsString().NotNullable()
                .WithColumn("content_hash").AsString().NotNullable()
                .WithColumn("indexed_at").AsDateTime().NotNullable();

            Create.Index("ux_files_repository_path")
                .OnTable("files")
                .OnColumn("repository_id").Ascending()
                .OnColumn("relative_path").Ascending()
                .WithOptions().Unique();

            Create.Table("chunks")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("file_id").AsInt32().NotNullable()
                    .ForeignKey("fk_chunks_files", "files", "id").OnDelete(Rule.Cascade)
                .WithColumn("chunk_index").AsInt32().NotNullable()
                .WithColumn("start_line").AsInt32().NotNullable()
                .WithColumn("end_line").AsInt32().NotNullable()
                .WithColumn("symbol").AsString().Nullable()
                .WithColumn("content").AsString().NotNullable();

            Create.Index("ix_chunks_file_id")
                .OnTable("chunks")
                .OnColumn("file_id").Ascending();

            Create.Table("embeddings")
                .WithColumn("chunk_id").AsInt32().PrimaryKey()
                    .ForeignKey("fk_embeddings_chunks", "chunks", "id").OnDelete(Rule.Cascade)
                .WithColumn("model_name").AsString().NotNullable()
                .WithColumn("dimensions").AsInt32().NotNullable()
                .WithColumn("vector").AsBinary().NotNullable();
        }
    }
}
