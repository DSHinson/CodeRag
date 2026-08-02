using CodeRag.Common.Results;
using FluentMigrator.Runner;

namespace CodeRag.db.Migrations
{
    public sealed class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IMigrationRunner _runner;

        public DatabaseInitializer(IMigrationRunner runner)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        public Result Initialize()
        {
            try
            {
                _runner.MigrateUp();
                return ResultHelpers.ToResult();
            }
            catch (Exception ex)
            {
                return ResultHelpers.ToFailure(ex.Message);
            }
        }
    }
}
