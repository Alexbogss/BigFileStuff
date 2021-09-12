namespace BigFileStuff.SorterUtility.Sorting.Splitting
{
    public static class FileSplitterFactory
    {
        public static IFileSplitter GetLineFileSplitter(string temp) => new LineFileSplitter(temp);
    }
}
