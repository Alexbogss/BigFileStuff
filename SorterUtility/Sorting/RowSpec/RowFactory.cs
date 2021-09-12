namespace BigFileStuff.SorterUtility.Sorting.RowSpec
{
    public static class RowFactory
    {
        public static T InitRow<T>(string lineText, int streamReader = 0) where T : IRow, new()
        {
            var row = new T();
            row.Init(lineText);
            return row;
        }
    }
}
