namespace CodeRag.bl.Indexing
{
    /// <summary>
    /// A contiguous slice of a source file, produced by the chunker.
    /// Holds text and location only; tokens are produced later by the embedder.
    /// </summary>
    /// <param name="FilePath">Absolute path of the file the chunk came from.</param>
    /// <param name="StartLine">1-based first line of the chunk.</param>
    /// <param name="EndLine">1-based last line of the chunk (inclusive).</param>
    /// <param name="Content">The raw source text of the chunk.</param>
    /// <param name="Symbol">First declaration-looking line in the chunk, or null if none.</param>
    public sealed record CodeChunk(
        string FilePath,
        int StartLine,
        int EndLine,
        string Content,
        string? Symbol);
}
