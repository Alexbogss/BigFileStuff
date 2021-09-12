using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BigFileStuff.SorterUtility.Sorting.Splitting
{
    public class LineFileSplitter : IFileSplitter
    {
        private readonly string _tempFileLocation;

        public LineFileSplitter(string tempFileLocation)
        {
            _tempFileLocation = tempFileLocation;

            if (!Directory.Exists(_tempFileLocation))
            {
                Directory.CreateDirectory(_tempFileLocation);
            }
        }

        public async Task<IReadOnlyList<string>> SplitFile(Stream source)
        {
            var lineBuffer = new string[Common.SplitFileLineCount];
            var filenames = new List<string>();

            using var reader = new StreamReader(source);
            long currentFile = 0;

            while (!reader.EndOfStream)
            {
                var linesReadCount = 0;
                while (linesReadCount < Common.SplitFileLineCount)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) break;

                    lineBuffer[linesReadCount++] = line;
                }

                var filename = $"{++currentFile}{Common.UnsortedFileExtension}";
                var filePath = Path.Combine(_tempFileLocation, filename);
                await using var unsortedFile = new StreamWriter(filePath);

                foreach (var line in lineBuffer)
                {
                    await unsortedFile.WriteLineAsync(line);
                }

                filenames.Add(filename);
            }

            return filenames;
        }
    }
}
