using CodeRag.Common.Results;

namespace CodeRag.bl.Model
{
    public interface IModelInfrastructureService
    {
        public sealed record ModelPaths(string ModelPath, string VocabPath);

        ModelPaths GetPaths(EmbeddingModelInfo model);
        bool IsModelAvailable(EmbeddingModelInfo model);
        Task<Result> EnsureModelDownloadedAsync(EmbeddingModelInfo model, IProgress<double>? progress = null);
    }
}
