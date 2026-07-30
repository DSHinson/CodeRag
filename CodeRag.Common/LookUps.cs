namespace CodeRag.Common
{
    public static class LookUps
    {
        public static class StorageLocations
        {
            public static readonly string RootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CodeRag");
            public static readonly string DbPath = Path.Combine(RootPath, "coderag.db");
        }
    }
}
