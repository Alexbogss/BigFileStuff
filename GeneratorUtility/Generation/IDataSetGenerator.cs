namespace BigFileStuff.GeneratorUtility.Generation
{
    public class DataSetGeneratorResult
    {
        public string[] StringValues { get; set; }

        public long TotalRowCount { get; set; }
    }

    public interface IDataSetGenerator
    {
        public DataSetGeneratorResult Generate(long size);
    }
}
