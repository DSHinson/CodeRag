using CodeRag.Common.Results;

namespace CodeRag.db.Store
{
    /// <summary>
    /// Represents a SQL store that provides methods for executing SQL commands and queries asynchronously.
    /// </summary>
    public interface ISqlStore
    {
        Task<Result<int>> ExecuteAsync(string sql, object? parameters = null);
        Task<Result<T?>> QuerySingleAsync<T>(string sql, object? parameters = null);
        Task<Result<IReadOnlyList<T>>> QueryAsync<T>(string sql, object? parameters = null);
        Task<Result<T?>> ExecuteScalarAsync<T>(string sql, object? parameters = null);
    }
}
