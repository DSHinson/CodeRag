namespace CodeRag.bl.Indexing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ChunkingOptions
    {
        /// <summary>A chunk smaller than this won't be flushed at a declaration boundary.</summary>
        public int MinTokens { get; init; } = 80;

        /// <summary>The size the chunker packs paragraphs up to.</summary>
        public int TargetTokens { get; init; } = 200;

        /// <summary>A single paragraph larger than this is split by lines.</summary>
        public int MaxTokens { get; init; } = 240;

        /// <summary>Lines repeated at the start of the next window when splitting an oversized block.</summary>
        public int OverlapLines { get; init; } = 3;
    }
}
