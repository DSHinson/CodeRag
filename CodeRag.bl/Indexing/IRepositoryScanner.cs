using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.bl.Indexing
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRepositoryScanner
    {
        int CountFiles(string repositoryPath);
    }
}
