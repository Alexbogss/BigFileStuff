using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigFileStuff.SorterUtility.Sorting.RowSpec;

namespace BigFileStuff.SorterUtility.Sorting
{
    public static class FileSorterFactory
    {
        public static IFileSorter<T> GetExternalMergeSorter<T>() where T : IRow, new()
            => new ExternalMergeSorter<T>();
    }
}
