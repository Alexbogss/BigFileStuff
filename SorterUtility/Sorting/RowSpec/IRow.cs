using System;

namespace BigFileStuff.SorterUtility.Sorting.RowSpec
{
    public interface IRow : IComparable
    {
        public string Text { get; }

        public int StreamReader { get; }

        public void Init(string rowText, int streamReaderId = 0);
    }
}
