using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BigFileStuff.SorterUtility.Sorting;
using BigFileStuff.SorterUtility.Sorting.RowSpec;
using BigFileStuff.SorterUtility.Sorting.Splitting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SortingTests
    {
        [TestMethod]
        public async Task TestFileSplit()
        {
            const string path = "./test.txt";
            const string tempLocation = "./temp";

            var sw = new Stopwatch();
            sw.Start();

            var splitter = new LineFileSplitter(tempLocation);
            var files = await splitter.SplitFile(File.OpenRead(path));

            sw.Stop();

            Trace.WriteLine($"File splitted to {files.Count} parts in {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public async Task TestFileSort()
        {
            const string path = "./test.txt";

            var sorter = new ExternalMergeSorter<CaseRow>();

            var sw = new Stopwatch();

            sw.Start();
            await sorter.Sort(File.OpenRead(path));
            sw.Stop();

            Trace.WriteLine($"Sort elapsed time {sw.ElapsedMilliseconds}ms");
        }
    }
}
