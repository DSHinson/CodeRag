using CodeRag.Common.Results;

namespace CodeRag.db.Migrations
{
    public interface IDatabaseInitializer
    {
        Result Initialize();
    }
}
