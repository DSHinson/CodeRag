using CodeRag.Common.Results;
using Microsoft.Data.Sqlite;
using Dapper;

namespace CodeRag.db.Store
{
    public sealed class SqlliteStore : ISqlStore, IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public SqlliteStore(string databasePath)
        {
            ArgumentNullException.ThrowIfNull(databasePath);

            _connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                ForeignKeys = true
            }.ToString());

            _connection.Open();

            using(var pragma = _connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA busy_timeout = 5000;";
                pragma.ExecuteNonQuery();
            }
        }

        public async ValueTask DisposeAsync()
        {
            _gate.Dispose();
            await _connection.DisposeAsync();
        }

        public Task<Result<int>> ExecuteAsync(string sql, object? parameters = null) => RunAsync(conn => conn.ExecuteAsync(sql, parameters));

        public Task<Result<T?>> QuerySingleAsync<T>(string sql, object? parameters = null) => RunAsync(conn => conn.QuerySingleOrDefaultAsync<T>(sql, parameters));

        public Task<Result<IReadOnlyList<T>>> QueryAsync<T>(string sql, object? parameters = null) => RunAsync<IReadOnlyList<T>>(async conn => (await conn.QueryAsync<T>(sql, parameters)).ToList());

        public Task<Result<T?>> ExecuteScalarAsync<T>(string sql, object? parameters = null) => RunAsync(conn => conn.ExecuteScalarAsync<T>(sql, parameters));

        /// <summary>
        /// Executes the provided work function within a semaphore lock to ensure thread safety when accessing the SQLite connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <returns></returns>
        private async Task<Result<T>> RunAsync<T>(Func<SqliteConnection, Task<T>> work)
        {
            await _gate.WaitAsync();
            try
            {
                var value = await work(_connection);
                return ResultHelpers.ToResult(value);
            }
            catch (Exception ex)
            {
                return ResultHelpers.ToFailure<T>(ex.Message);
            }
            finally
            {
                _gate.Release();
            }
        }
    }
}
