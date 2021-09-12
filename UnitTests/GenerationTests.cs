using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BigFileStuff.GeneratorUtility.Generation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GenerationTests
    {
        [TestMethod]
        public void TestDataSetGeneration()
        {
            const long size = 200_000_000_000;

            var dataSetGenerator = DataSetGeneratorFactory.GetDataSetGenerator();

            var sw = new Stopwatch();

            sw.Start();
            var dataSet = dataSetGenerator.Generate(size);
            sw.Stop();

            Assert.IsTrue(dataSet.StringValues.Any());

            Trace.WriteLine($"Elapsed time for {size} bytes: {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public async Task TestBigFileGenerate()
        {
            const long size = 500_000_000;
            const string path = "./test.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var dataSetGenerator = DataSetGeneratorFactory.GetDataSetGenerator();
            var dataSet = dataSetGenerator.Generate(size);

            var sw = new Stopwatch();

            sw.Start();

            var writer = new GeneratorFileWriter(dataSet.StringValues);
            await writer.GenerateAndWriteData(path, dataSet.TotalRowCount);

            sw.Stop();

            Assert.IsTrue(File.Exists(path));

            Trace.WriteLine($"Elapsed time for {size} bytes: {sw.ElapsedMilliseconds}ms");

            //File.Delete(path);
        }
    }
}
