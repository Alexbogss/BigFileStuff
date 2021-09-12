using System;
using System.IO;
using System.Threading.Tasks;

namespace BigFileStuff.GeneratorUtility.Generation
{
    public class GeneratorFileWriter
    {
        private readonly string[] _stringValues;

        public GeneratorFileWriter(string[] stringValues)
        {
            _stringValues = stringValues;
        }

        public async Task GenerateAndWriteData(string path, long rowCount)
        {
            await using var file = new StreamWriter(path);

            var random = new Random();

            for (int i = 0; i < rowCount; i++)
            {
                await file.WriteLineAsync(GetRow(random));
            }
        }

        private string GetRow(Random random)
        {
            var numberValue = random.Next();
            var stringValue = _stringValues[random.Next(_stringValues.Length)];

            return $"{numberValue}. {stringValue}";
        }
    }
}
