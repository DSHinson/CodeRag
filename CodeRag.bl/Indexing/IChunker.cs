namespace CodeRag.bl.Indexing
{
    public interface IChunker
    {
        IReadOnlyList<CodeChunk> Chunk(string filePath, string content);
    }
}
