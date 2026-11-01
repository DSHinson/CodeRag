namespace CodeRag.bl.Indexing
{
    public interface ITokenCounter
    {
        int Count(string text);
    }

    /// <summary>
    /// Cheap stand-in until the real tokenizer is wired in.
    /// Source code averages roughly 3 characters per token, which errs on the safe side.
    /// </summary>
    public sealed class EstimatedTokenCounter : ITokenCounter
    {
        public int Count(string text)
        {
            return (int)Math.Ceiling(text.Length / 3.0);
        }
    }
}
