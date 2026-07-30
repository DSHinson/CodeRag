namespace CodeRag.Common.Results
{
    /// <summary>
    /// Represents the base type for the outcome of an operation, providing a common abstraction for success and failure
    /// results.
    /// </summary>
    /// <remarks>Use derived types such as <see cref="Result.Success"/> to indicate specific outcomes. This type should be used for success responsess where there is no return data</remarks>
    public abstract record Result
    {
        public sealed record Success() : Result;
        public sealed record Failure(string FailureMessage) : Result;
    }
    /// <summary>
    /// Represents the result of an operation that can succeed with a value or fail with an error message.
    /// </summary>
    /// <remarks>Use this type to model operations that may either succeed and return a value, or fail and
    /// provide an error message. The result is represented by one of two derived records: <see
    /// cref="Result{T}.Success"/> for successful outcomes, and <see cref="Result{T}.Failure"/> for failures. This
    /// pattern enables explicit handling of both success and failure cases without relying on exceptions.</remarks>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    public abstract record Result<T>
    {
        private Result()
        {
            if (typeof(Result).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException("T cannot be a Result type.");
            }
        }
        public sealed record Success(T Data) : Result<T>;
        public sealed record Failure(string FailureMessage) : Result<T>;

    }

    /// <summary>
    /// Provides extension methods for creating success and failure results from data or error messages.
    /// </summary>
    /// <remarks>These helper methods simplify the creation of <see cref="Result{T}"/> instances, allowing for
    /// concise and readable code when handling operation outcomes. The methods are intended to be used with types that
    /// represent the result of an operation, encapsulating either a successful value or a failure message.</remarks>
    public static class ResultHelpers
    {
        public static Result ToResult() => new Result.Success();
        public static Result<T> ToResult<T>(this T data) => new Result<T>.Success(data);
        public static Result ToFailure(this string failureMessage) => new Result.Failure(failureMessage);
        public static Result<T> ToFailure<T>(this string failureMessage) => new Result<T>.Failure(failureMessage);

    }
}
