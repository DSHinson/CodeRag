namespace CodeRag.bl.Indexing
{
    public sealed class RepositoryScanner : IRepositoryScanner
    {
        private static readonly HashSet<string> ExcludedDirs = new(StringComparer.OrdinalIgnoreCase)
        {
            ".git", "node_modules", "bin", "obj", "dist", "build", ".vs", ".idea"
        };

        public int CountFiles(string repositoryPath)
        {
            if (!Directory.Exists(repositoryPath))
            {
                return 0;
            }

            return CountRecursive(repositoryPath);
        }

        private int CountRecursive(string dir, int depth = 0)
        {
            int count = 0;
            if (depth > 1000)
            {
                // Prevent stack overflow in case of very deep directory structures
                return count;
            }
            try
            {
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    count++;
                }

                foreach (var subDir in Directory.EnumerateDirectories(dir))
                {
                    var name = Path.GetFileName(subDir);
                    if (!ExcludedDirs.Contains(name))
                    {
                        count += CountRecursive(subDir, depth + 1);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // skip directories we can't read rather than failing the whole count
            }

            return count;
        }
    }
}
