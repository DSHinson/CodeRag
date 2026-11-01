using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeRag.bl.Indexing
{
    public sealed class CodeChunker : IChunker
    {
        private static readonly Regex DeclarationPattern = new(
           @"^\s*(?:public|private|protected|internal|static|abstract|sealed|override|virtual|partial|export|class|interface|struct|enum|record|namespace|trait|impl|def|function|func|fn)\s",
           RegexOptions.Compiled);

        private static readonly string[] CommentOrAttributePrefixes = {"//", "/*", "*", "[", "@", "#"};
        private readonly ITokenCounter _tokens;
        private readonly ChunkingOptions _options;

        public CodeChunker(ITokenCounter tokens, ChunkingOptions options)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IReadOnlyList<CodeChunk> Chunk(string filePath, string content)
        {
            throw new NotImplementedException();
        }
    }
}
