using CodeRag.Common;
using CodeRag.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using static CodeRag.bl.Model.IModelInfrastructureService;

namespace CodeRag.bl.Model
{
    public sealed class ModelInfrastructureService : IModelInfrastructureService
    {
        private readonly HttpClient _httpClient;

        public ModelInfrastructureService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        public Task<Result> EnsureModelDownloadedAsync(EmbeddingModelInfo model, IProgress<double>? progress = null)
        {
            throw new NotImplementedException();
        }

        public IModelInfrastructureService.ModelPaths GetPaths(EmbeddingModelInfo model)
        {
            string modelDir = Path.Combine(LookUps.StorageLocations.ModelsPath, model.Name);

            return new ModelPaths( ModelPath: Path.Combine(modelDir, model.ModelFileName),VocabPath: Path.Combine(modelDir, model.VocabFileName));
        }

        public bool IsModelAvailable(EmbeddingModelInfo model)
        {
            ModelPaths paths = GetPaths(model);
            return File.Exists(paths.ModelPath) && File.Exists(paths.VocabPath);
        }
    }
}
