using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace CodeRag.db.Migrations
{
    public static class MigrationServiceCollectionExtensions
    {
        public static IServiceCollection AddCodeRagMigrations(this IServiceCollection services, string connectionString)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(CreateRepositoriesTable).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            return services;
        }
    }
}