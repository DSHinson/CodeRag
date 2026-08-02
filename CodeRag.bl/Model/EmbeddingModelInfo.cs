namespace CodeRag.bl.Model
{
    /// <summary>
    /// Represents information about an embedding model.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="ModelFileName"></param>
    /// <param name="VocabFileName"></param>
    /// <param name="OutputDimensions"></param>
    public sealed record EmbeddingModelInfo(string Name, string ModelFileName, string VocabFileName, int OutputDimensions);

    /// <summary>
    /// Contains predefined embedding model information.
    /// </summary>
    public static class EmbeddingModels
    {
        /// <summary>
        /// Represents the "all-MiniLM-L6-v2" embedding model information.
        /// </summary>
        public static readonly EmbeddingModelInfo AllMiniLmL6V2 = new( Name: "all-MiniLM-L6-v2", ModelFileName: "model.onnx", VocabFileName: "vocab.txt",
            OutputDimensions: 384);
    }
}
