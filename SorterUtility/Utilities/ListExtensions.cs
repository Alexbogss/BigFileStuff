using System.Collections.Generic;
using System.Linq;

namespace BigFileStuff.SorterUtility.Utilities
{
    public static class ListExtensions
    {
        public static List<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
