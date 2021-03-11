using System;
using System.Collections.Generic;
using GSF.IO.Unmanaged;
using NUnit.Framework;
using openHistorian.Snap;

namespace GSF.Snap.Filters.Test
{
    [TestFixture]
    public class TimestampFilterTest
    {
        [Test]
        public void TestFixedRange()
        {
            _ = new List<ulong>();
            SeekFilterBase<HistorianKey> pointId = TimestampSeekFilter.CreateFromRange<HistorianKey>(0, 100);

            if (!pointId.GetType().FullName.Contains("FixedRange"))
                throw new Exception("Wrong type");

            using (BinaryStream bs = new BinaryStream(allocatesOwnMemory: true))
            {
                bs.Write(pointId.FilterType);
                pointId.Save(bs);
                bs.Position = 0;

                SeekFilterBase<HistorianKey> filter = Library.Filters.GetSeekFilter<HistorianKey>(bs.ReadGuid(), bs);

                if (!filter.GetType().FullName.Contains("FixedRange"))
                    throw new Exception("Wrong type");
            }
        }

        [Test]
        public void TestIntervalRanges()
        {
            _ = new List<ulong>();
            SeekFilterBase<HistorianKey> pointId = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(0, 100, 10, 3, 1);

            if (!pointId.GetType().FullName.Contains("IntervalRanges"))
                throw new Exception("Wrong type");

            using (BinaryStream bs = new BinaryStream(allocatesOwnMemory: true))
            {
                bs.Write(pointId.FilterType);
                pointId.Save(bs);
                bs.Position = 0;

                SeekFilterBase<HistorianKey> filter = Library.Filters.GetSeekFilter<HistorianKey>(bs.ReadGuid(), bs);

                if (!filter.GetType().FullName.Contains("IntervalRanges"))
                    throw new Exception("Wrong type");
            }
        }

    }
}
