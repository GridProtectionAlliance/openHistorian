using System;
using System.Collections.Generic;
using GSF.IO.Unmanaged;
using NUnit.Framework;
using openHistorian.Snap;

namespace GSF.Snap.Filters.Test
{
    [TestFixture]
    public class PointIDFilterTest
    {
        [Test]
        public void TestBitArray()
        {
            List<ulong> list = new List<ulong>();
            MatchFilterBase<HistorianKey, HistorianValue> pointId = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(list);

            if (!pointId.GetType().FullName.Contains("BitArrayFilter"))
                throw new Exception("Wrong type");

            using (BinaryStream bs = new BinaryStream(allocatesOwnMemory: true))
            {
                bs.Write(pointId.FilterType);
                pointId.Save(bs);
                bs.Position = 0;

                MatchFilterBase<HistorianKey, HistorianValue> filter = Library.Filters.GetMatchFilter<HistorianKey, HistorianValue>(bs.ReadGuid(), bs);

                if (!filter.GetType().FullName.Contains("BitArrayFilter"))
                    throw new Exception("Wrong type");
            }
        }

        [Test]
        public void TestUintHashSet()
        {
            List<ulong> list = new List<ulong>();
            list.Add(132412341);
            MatchFilterBase<HistorianKey, HistorianValue> pointId = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(list);

            if (!pointId.GetType().FullName.Contains("UIntHashSet"))
                throw new Exception("Wrong type");

            using (BinaryStream bs = new BinaryStream(allocatesOwnMemory: true))
            {
                bs.Write(pointId.FilterType);
                pointId.Save(bs);
                bs.Position = 0;

                MatchFilterBase<HistorianKey, HistorianValue> filter = Library.Filters.GetMatchFilter<HistorianKey, HistorianValue>(bs.ReadGuid(), bs);

                if (!filter.GetType().FullName.Contains("UIntHashSet"))
                    throw new Exception("Wrong type");
            }
        }

        [Test]
        public void TestUlongHashSet()
        {
            List<ulong> list = new List<ulong>();
            list.Add(13242345234523412341ul);
            MatchFilterBase<HistorianKey, HistorianValue> pointId = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(list);

            if (!pointId.GetType().FullName.Contains("ULongHashSet"))
                throw new Exception("Wrong type");

            using (BinaryStream bs = new BinaryStream(allocatesOwnMemory: true))
            {
                bs.Write(pointId.FilterType);
                pointId.Save(bs);
                bs.Position = 0;

                MatchFilterBase<HistorianKey, HistorianValue> filter = Library.Filters.GetMatchFilter<HistorianKey, HistorianValue>(bs.ReadGuid(), bs);

                if (!filter.GetType().FullName.Contains("ULongHashSet"))
                    throw new Exception("Wrong type");
            }
        }

    }
}
