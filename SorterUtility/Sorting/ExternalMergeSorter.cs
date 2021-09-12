using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BigFileStuff.SorterUtility.Sorting.RowSpec;
using BigFileStuff.SorterUtility.Sorting.Splitting;
using BigFileStuff.SorterUtility.Utilities;

namespace BigFileStuff.SorterUtility.Sorting
{
    public class ExternalMergeSorter<T> : IFileSorter<T> where T : IRow, new()
    {
        // TODO to options
        private const string TempFileLocation = ".\\temp";
        private const string TargetFileLocation = ".\\sorted.txt";

        public async Task Sort(Stream source)
        {
            var splitter = FileSplitterFactory.GetLineFileSplitter(TempFileLocation);
            var files = await splitter.SplitFile(source);

            var sortedFiles = await SortFiles(files);
            await MergeFiles(sortedFiles, File.OpenWrite(TargetFileLocation));
        }

        private static async Task<IReadOnlyList<string>> SortFiles(IReadOnlyList<string> unsortedFiles)
        {
            var sortedFilenames = new List<string>(unsortedFiles.Count);

            foreach (var unsortedFile in unsortedFiles)
            {
                var sortedFilename = unsortedFile.Replace(
                    Common.UnsortedFileExtension,
                    Common.SortedFileExtension);
                var unsortedFilePath = Path.Combine(TempFileLocation, unsortedFile);
                var sortedFilePath = Path.Combine(TempFileLocation, sortedFilename);

                await SortFile(unsortedFilePath, sortedFilePath);

                File.Delete(unsortedFilePath);

                sortedFilenames.Add(sortedFilename);
            }

            return sortedFilenames;
        }

        private static async Task SortFile(string unsortedFilePath, string targetFilePath)
        {
            static async Task<List<T>> ReadFile(string filePath)
            {
                var result = new List<T>(Common.SplitFileLineCount);
                using var reader = new StreamReader(filePath);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    result.Add(RowFactory.InitRow<T>(line));
                }

                return result;
            }

            var rows = await ReadFile(unsortedFilePath);
            rows.Sort();

            await using var streamWriter = new StreamWriter(targetFilePath);
            foreach (var row in rows)
            {
                await streamWriter.WriteLineAsync(row.Text);
            }
        }

        private static async Task MergeFiles(IReadOnlyList<string> sortedFiles, Stream target)
        {
            const int filesPerRun = 10;

            var done = false;
            while (!done)
            {
                var finalRun = sortedFiles.Count <= filesPerRun;

                if (finalRun)
                {
                    await Merge(sortedFiles, target);
                    return;
                }

                var runs = sortedFiles.Chunk(filesPerRun);
                var chunkCounter = 0;
                foreach (var files in runs)
                {
                    var outputFilename = $"{++chunkCounter}{Common.SortedFileExtension}{Common.TempFileExtension}";
                    if (files.Count == 1)
                    {
                        File.Move(GetFullPath(files.First()), GetFullPath(outputFilename.Replace(Common.TempFileExtension, string.Empty)));
                        continue;
                    }

                    var outputStream = File.OpenWrite(GetFullPath(outputFilename));
                    await Merge(files, outputStream);
                    File.Move(GetFullPath(outputFilename), GetFullPath(outputFilename.Replace(Common.TempFileExtension, string.Empty)), true);
                }

                sortedFiles = Directory.GetFiles(TempFileLocation, $"*{Common.SortedFileExtension}")
                    .OrderBy(x =>
                    {
                        var filename = Path.GetFileNameWithoutExtension(x);
                        return int.Parse(filename);
                    })
                    .Select(Path.GetFileName)
                    .ToArray();

                if (sortedFiles.Count > 1)
                {
                    continue;
                }

                done = true;
            }
        }

        private static async Task Merge(IReadOnlyList<string> filesToMerge, Stream outputStream)
        {
            var (streamReaders, rows) = await InitializeStreamReaders(filesToMerge);
            var finishedStreamReaders = new List<int>(streamReaders.Count);
            
            await using var outputWriter = new StreamWriter(outputStream);

            var done = false;
            while (!done)
            {
                rows.Sort();

                var valueToWrite = rows[0].Text;
                var streamReaderIndex = rows[0].StreamReader;
                await outputWriter.WriteLineAsync(valueToWrite.AsMemory());

                if (streamReaders[streamReaderIndex].EndOfStream)
                {
                    var indexToRemove = rows.FindIndex(x => x.StreamReader == streamReaderIndex);
                    rows.RemoveAt(indexToRemove);
                    finishedStreamReaders.Add(streamReaderIndex);
                    done = finishedStreamReaders.Count == streamReaders.Count;
                    continue;
                }

                var line = await streamReaders[streamReaderIndex].ReadLineAsync();
                rows[0] = RowFactory.InitRow<T>(line, streamReaderIndex);
            }

            CleanupRun(streamReaders, filesToMerge);
        }

        private static async Task<(IReadOnlyList<StreamReader> StreamReaders, List<T> rows)> InitializeStreamReaders(IReadOnlyList<string> sortedFiles)
        {
            var streamReaders = new StreamReader[sortedFiles.Count];
            var rows = new List<T>(sortedFiles.Count);
            for (var i = 0; i < sortedFiles.Count; i++)
            {
                var sortedFilePath = GetFullPath(sortedFiles[i]);
                var sortedFileStream = File.OpenRead(sortedFilePath);
                streamReaders[i] = new StreamReader(sortedFileStream);
                var lineText = await streamReaders[i].ReadLineAsync();
                rows.Add(RowFactory.InitRow<T>(lineText, i));
            }

            return (streamReaders, rows);
        }

        private static void CleanupRun(IReadOnlyList<StreamReader> streamReaders, IReadOnlyList<string> filesToMerge)
        {
            for (var i = 0; i < streamReaders.Count; i++)
            {
                streamReaders[i].Dispose();
                var temporaryFilename = $"{filesToMerge[i]}{Common.RemovalFileExtension}";
                File.Move(GetFullPath(filesToMerge[i]), GetFullPath(temporaryFilename));
                File.Delete(GetFullPath(temporaryFilename));
            }
        }

        private static string GetFullPath(string filename) => Path.Combine(TempFileLocation, filename);
    }
}
