namespace BigFileStuff.GeneratorUtility.Generation
{
    public static class DataSetGeneratorFactory
    {
        public static IDataSetGenerator GetDataSetGenerator()
        {
            return new RandomDataSetGenerator();
        }
    }
}
