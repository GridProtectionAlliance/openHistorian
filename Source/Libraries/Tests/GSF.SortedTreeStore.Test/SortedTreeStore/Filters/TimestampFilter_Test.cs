using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.IO.Unmanaged;
using NUnit.Framework;
using openHistorian.Collections;

namespace GSF.SortedTreeStore.Filters
{
    [TestFixture]
    public class TimestampFilter_Test
    {
        [Test]
        public void TestFixedRange()
        {
            var list = new List<ulong>();
            var pointId = TimestampFilter.CreateFromRange<HistorianKey>(0, 100);

            if (!pointId.GetType().FullName.Contains("FixedRange"))
                throw new Exception("Wrong type");

            using (var bs = new BinaryStream(allocatesOwnMemory: true))
            {
                bs.Write(pointId.FilterType);
                pointId.Save(bs);
                bs.Position = 0;

                var filter = Library.Filters.GetSeekFilter<HistorianKey>(bs.ReadGuid(), bs);

                if (!filter.GetType().FullName.Contains("FixedRange"))
                    throw new Exception("Wrong type");
            }
        }

        [Test]
        public void TestIntervalRanges()
        {
            var list = new List<ulong>();
            var pointId = TimestampFilter.CreateFromIntervalData<HistorianKey>(0, 100, 10, 3, 1);

            if (!pointId.GetType().FullName.Contains("IntervalRanges"))
                throw new Exception("Wrong type");

            using (var bs = new BinaryStream(allocatesOwnMemory: true))
            {
                bs.Write(pointId.FilterType);
                pointId.Save(bs);
                bs.Position = 0;

                var filter = Library.Filters.GetSeekFilter<HistorianKey>(bs.ReadGuid(), bs);

                if (!filter.GetType().FullName.Contains("IntervalRanges"))
                    throw new Exception("Wrong type");
            }
        }

    }
}
