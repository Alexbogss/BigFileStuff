using System.IO;
using System.Threading.Tasks;
using BigFileStuff.SorterUtility.Sorting.RowSpec;

namespace BigFileStuff.SorterUtility.Sorting
{
    public interface IFileSorter<in T> where T : IRow, new()
    {
        Task Sort(Stream source);
    }
}
