using System;
using System.Numerics;

namespace BigFileStuff.GeneratorUtility.Generation
{
    public class RandomDataSetGenerator : IDataSetGenerator
    {
        private const int StringLength = 8;

        // 12 bytes for text num, point and space to string 
        private const long TotalRowByteSize = StringLength + 12;

        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public DataSetGeneratorResult Generate(long size)
        {
            var uniqueStrCount = GetUniqueStringCount(size);
            var stringValues = new string[uniqueStrCount];

            var random = new Random();
            for (int i = 0; i < uniqueStrCount; i++)
            {
                stringValues[i] = GetRandomString(random, StringLength);
            }

            return new DataSetGeneratorResult
            {
                StringValues = stringValues,
                TotalRowCount = GetTotalRowCount(size)
            };
        }

        private static long GetUniqueStringCount(long size) => (long) BigInteger.Pow(4, size.ToString().Length);

        private static long GetTotalRowCount(long size) => size / TotalRowByteSize;

        private static string GetRandomString(Random rand, int length)
        {
            var buffer = new char[length];
            var charsLength = Chars.Length;

            for (int i = 0; i < length; i++)
            {
                buffer[i] = Chars[rand.Next(charsLength)];
            }

            return new string(buffer);
        }
    }
}
