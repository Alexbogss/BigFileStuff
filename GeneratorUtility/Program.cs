using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BigFileStuff.GeneratorUtility.Generation;
using BigFileStuff.GeneratorUtility.Utilities;
using CommandLine;

namespace BigFileStuff.GeneratorUtility
{
    internal sealed class Program
    {
        private static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async options =>
                {
                    try
                    {
                        await MainArgumentsHandler(options);
                    }
                    catch (Exception e)
                    {
                        await MainExceptionHandler(e);
                    }
                }, async errors =>
                {
                    await MainParseErrorHandler(errors);
                });
        }

        private static async Task MainArgumentsHandler(CommandLineOptions options)
        {
            var validationResult = CliArgsValidator.Validate(options);
            HandleValidateResult(validationResult);

            Console.WriteLine("Starting generate unique string data set");

            var sw = new Stopwatch();
            sw.Start();

            var dataSetGenerator = DataSetGeneratorFactory.GetDataSetGenerator();
            var dataSet = dataSetGenerator.Generate(options.Size);

            sw.Stop();
            Console.WriteLine($"Data set with {dataSet.StringValues.Length} " +
                              $"unique strings generated in {sw.ElapsedMilliseconds}ms");

            Console.WriteLine($"Starting generate {options.Path} file size {options.Size} bytes ({dataSet.TotalRowCount} rows)");
            sw.Reset();
            sw.Start();

            var writer = new GeneratorFileWriter(dataSet.StringValues);
            await writer.GenerateAndWriteData(options.Path, dataSet.TotalRowCount);

            Console.WriteLine($"Generate done in {sw.ElapsedMilliseconds}ms");
        }

        private static Task MainExceptionHandler(Exception exception)
        {
            Console.WriteLine($"Fatal error. Utility exception: {exception.Message}");
            return Task.CompletedTask;
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

        private static void HandleValidateResult(CliValidationResult validationResult)
        {
            if (!validationResult.Success)
            {
                foreach (var errText in validationResult.Errors)
                {
                    Console.WriteLine($"Validation error: {errText}");
                }

                throw new Exception("Fatal error: check arguments value and try again!");
            }
        }
    }
}
