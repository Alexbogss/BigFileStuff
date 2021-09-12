namespace BigFileStuff.SorterUtility.Sorting.RowSpec
{
    public class BaseRow : IRow
    {
        public string Text { get; private set; }

        public int StreamReader { get; private set; }

        public virtual void Init(string rowText, int streamReaderId = 0)
        {
            Text = rowText;
            StreamReader = streamReaderId;
        }

        public virtual int CompareTo(object? obj) => Text.CompareTo(obj);
    }
}
