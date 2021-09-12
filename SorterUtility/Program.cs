using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BigFileStuff.SorterUtility.Sorting;
using BigFileStuff.SorterUtility.Sorting.RowSpec;
using BigFileStuff.SorterUtility.Utilities;
using CommandLine;

namespace BigFileStuff.SorterUtility
{
    internal sealed class Program
    {
        private static async Task Main(string[] args)
        {
            // TODO handle cancellation token
            // TODO add cli progress bar
            await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async options =>
                {
                    try
                    {
                        ValidateCommandLineOptions(options);
                        await MainArgumentsHandler(options);
                    }
                    catch (Exception e)
                    {
                        MainExceptionHandler(e);
                    }
                }, async errors =>
                {
                    await MainParseErrorHandler(errors);
                });
        }

        private static void ValidateCommandLineOptions(CommandLineOptions options)
        {
            if (!File.Exists(options.Path))
            {
                throw new Exception($"Path validate error: File doesn't exists on selected path");
            }
        }

        private static async Task MainArgumentsHandler(CommandLineOptions options)
        {
            Console.WriteLine($"Starting sort file {options.Path}");

            var sw = new Stopwatch();
            sw.Start();

            var sorter = FileSorterFactory.GetExternalMergeSorter<CaseRow>();
            await sorter.Sort(File.OpenRead(options.Path));

            sw.Stop();
            Console.WriteLine($"Sort done in {sw.ElapsedMilliseconds}ms");
        }

        private static void MainExceptionHandler(Exception exception)
        {
            Console.WriteLine($"Fatal error. Utility exception: {exception.Message}");
        }

        private static Task MainParseErrorHandler(IEnumerable<Error> errors)
        {
            Console.WriteLine("Fatal error: arguments parse errors");
            foreach (var errorText in errors)
            {
                Console.WriteLine($"Parse error: {errorText}");
            }

            return Task.CompletedTask;
        }
    }
}
