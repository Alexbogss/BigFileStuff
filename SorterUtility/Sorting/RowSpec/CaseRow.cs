using System;

namespace BigFileStuff.SorterUtility.Sorting.RowSpec
{
    public class CaseRow : BaseRow
    {
        public int Number { get; private set; }
        public string Value { get; private set; }

        public override void Init(string rowText, int streamReaderId = 0)
        {
            base.Init(rowText, streamReaderId);

            var parts = rowText.Split(". ");
            Number = int.Parse(parts[0]);
            Value = parts[1];
        }

        public override int CompareTo(object? obj)
        {
            if (obj == null) return 1;

            if (!(obj is CaseRow row))
                throw new ArgumentException("Incomparable type");

            var valueCompareResult = Value.CompareTo(row.Value);

            return valueCompareResult == 0 
                ? Number.CompareTo(row.Number) 
                : valueCompareResult;
        }
    }
}
