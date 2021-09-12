using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BigFileStuff.SorterUtility.Sorting.Splitting
{
    public interface IFileSplitter
    {
        Task<IReadOnlyList<string>> SplitFile(Stream source);
    }
}
